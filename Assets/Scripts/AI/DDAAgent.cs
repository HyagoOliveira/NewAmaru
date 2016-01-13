using ALEPP;
using Minijogos;
using System.Collections.Generic;
using UnityEngine;

public class DDAAgent
{
    public const int NUMERO_REGRESSOES_PARA_REDUCAO_COMPARACOES = 2;
    public const float TAXA_CRESCIMENTO_DIFICULDADE = 0.05f;
    public const int INTERVALO_NORMALIZACAO = 3;

    private Stack<TarefaAprendizado> bufferTarefasPendentes;
    private TarefaAprendizado lastTarefaAprendizado;
    private Minijogo[] minijogos;

    public DDAAgent(params MinijogoType[] tiposMinijogos)
    {
        bufferTarefasPendentes = new Stack<TarefaAprendizado>();
        minijogos = new Minijogo[tiposMinijogos.Length];
        for (int i = 0; i < minijogos.Length; i++)
        {
            minijogos[i] = new Minijogo(tiposMinijogos[i]);
        }
    }

    public void Run(TarefaAprendizado tarefaAprendizado)
    {
        GerenciadorTarefas.Instance.Log("\tAgente DDA sendo executado...");
        lastTarefaAprendizado = tarefaAprendizado;        

        if (IsPossivelRegressao())
        {
            GerenciadorTarefas.Instance.Log("\tRegressao sendo feita...");
            SaveTarefaOnBuffer();

            CreateTarefaDDA();
            //REGRA DDA #2: Monitorar quantas regressões foram feita na tarefa tendo como base a primeira.
            lastTarefaAprendizado.numRegressoes++;
            GerenciadorTarefas.Instance.Log("\tRegressao realizada.");
        }
        else
        {
            GerenciadorTarefas.Instance.Log("\tRegressao nao eh possivel.");

            if (IsTarefaAprendizadoPendente())
            {
                lastTarefaAprendizado = GetTarefaAprendizadoPendente();
            }
            else if(lastTarefaAprendizado.comparacoes.Length > 2)
            {
                ReduzirComparacoes();
            }
            else
            {
                lastTarefaAprendizado = GerenciadorTarefas.Instance.NextTarefaAprendizado();
            }
        }        
    }

    public TarefaAprendizado GetTarefaAprendizado()
    {
        return lastTarefaAprendizado;
    }

    public TarefaAprendizado GetTarefaAprendizadoPendente()
    {
        TarefaAprendizado tarefa = bufferTarefasPendentes.Pop();
        GerenciadorTarefas.Instance.Log("\tTarefa " + tarefa.tipoTarefa.nome +
            " pendente armazenada no Buffer sendo retirada.");
        return tarefa;
    }

    public bool IsTarefaAprendizadoPendente()
    {
        return bufferTarefasPendentes.Count > 0;
    }

    public void AtualizaConhecimentoPalavra(TarefaAprendizado tarefa)
    {
        lastTarefaAprendizado = tarefa;

        CalculaDificuldades();

        GerenciadorTarefas.Instance.Log("\tAtualizando conhecimento da palavra modelo");
        //REGRA DDA #4: Atualizar o Conhecimento da Palavra analisando os 2 
        //últimos acertos consecutivos de tarefas contendo o mesmo Modelo de Palavra.
        float[] dificuldades = GerenciadorTarefas.Instance.
            Data.GetDificuldadePalavrasConsecultivas(lastTarefaAprendizado.modelo.nome);
        if(dificuldades.Length < 2)
        {
            GerenciadorTarefas.Instance.Log("\t\tNao foram realizados tarefas suficientes para analise do conhecimento dessa palavra");
            return;
        }

        int maxDificuldadeIndex = 0;
        float media = 0;
        for(int i=1; i < dificuldades.Length; i++)
        {
            media += dificuldades[i];
            if(dificuldades[i] > dificuldades[maxDificuldadeIndex])
            {
                maxDificuldadeIndex = i;
            }
        }
        GerenciadorTarefas.Instance.Log(string.Format(
            "\t\tConhecimento da Palavra: {0}, Maior Conhecimento: {1}, Media dos conhecimentos: {2}",
            lastTarefaAprendizado.modelo.conhecimentoGeral, dificuldades[maxDificuldadeIndex], media));
        //Se o Conhecimento atual da Palavra for menor que a tarefa onde o valor da 
        //Dificuldade Dinâmica seja maior, o Conhecimento será a média dos valores de Dificuldade Dinâmica.
        if(lastTarefaAprendizado.modelo.conhecimentoGeral < dificuldades[maxDificuldadeIndex])
        {
            lastTarefaAprendizado.modelo.conhecimentoGeral = media;
            GerenciadorTarefas.Instance.Log("\t\tConhecimento atualizado para " + 
                lastTarefaAprendizado.modelo.conhecimentoGeral);
        }
    }

    private float DificuldadeDinamica(float dificuldadeEstatica)
    {
        float conhecimentoPalavra = NormalizaMinMax(lastTarefaAprendizado.modelo.conhecimentoGeral, INTERVALO_NORMALIZACAO);

        float dd = 1f / (1f + Mathf.Exp(conhecimentoPalavra - dificuldadeEstatica));

        return dd;
    }

    private float DificuldadeEstatica()
    {
    	float somaFatorProximidade = 0f;
        float numeroComparacoes = lastTarefaAprendizado.comparacoes.Length;
        float pesoTipoTarefa = lastTarefaAprendizado.tipoTarefa.peso;
        //somatório de proximidades das comparações
        for (int i = 0; i < numeroComparacoes; i++)
        {
            if (lastTarefaAprendizado.modelo.nome.Equals(lastTarefaAprendizado.comparacoes[i].nome))
                continue; //é o próprio modelo: Nao deve soma-lo.

            float proximidade = GerenciadorTarefas.Instance.Data.GetProximidadePalavra(lastTarefaAprendizado.modelo.nome,
                lastTarefaAprendizado.comparacoes[i].nome);
            somaFatorProximidade += proximidade;
        }

        somaFatorProximidade /= (numeroComparacoes - 1f);

        return somaFatorProximidade + (1f / (numeroComparacoes + TAXA_CRESCIMENTO_DIFICULDADE)) + pesoTipoTarefa;
    }

    public void AtualizarRankMinijogo(TarefaAprendizado tarefa)
    {
        GerenciadorTarefas.Instance.Log("\tAtualizando Rank de Minijogos...");
        //REGRA DDA #3: Ordenar os Tipos de Minijogos de acordo com o Tempo de Latência associado a ele.
        for (int i = 0; i < minijogos.Length; i++)
        {
            if(tarefa.tipoMinijogo == minijogos[i].tipo)
            {
                minijogos[i].rank += (int)Mathf.Ceil(tarefa.TempoLatencia);
                break;
            }
        }

        tarefa.tipoMinijogo = GetBestMinijogo();
    }

    private MinijogoType GetBestMinijogo()
    {
        string log = "\t\tMinijogos -> [";
        int bestIndex = 0;
        //Quanto menor o rank, mais rápido o aluno concluir aquele minijogo.
        for (int i = 0; i < minijogos.Length; i++)
        {
            log += minijogos[i] + "; ";

            if(minijogos[i].rank < minijogos[bestIndex].rank)
            {
                bestIndex = i;
            }
        }

        log += "]";
        log += ", Melhor: " + minijogos[bestIndex];
        GerenciadorTarefas.Instance.Log(log);

        return minijogos[bestIndex].tipo;
    }

    private float NormalizaMinMax(float valor, float intervalo)
    {
        return valor * 2 * intervalo - intervalo;
    }

    private float DesnormalizaMinMax(float valor, float intervalo)
    {
        return (valor + intervalo) / (2 * intervalo);
    }

    private void CalculaDificuldades()
    {
        lastTarefaAprendizado.dificuldadeEstatica = DificuldadeEstatica();
        lastTarefaAprendizado.dificuldadeDinamica = DificuldadeDinamica(lastTarefaAprendizado.dificuldadeEstatica);

        GerenciadorTarefas.Instance.Log("Dificuldades: DE = " + lastTarefaAprendizado.dificuldadeEstatica + 
            ", DD = " + lastTarefaAprendizado.dificuldadeDinamica);
    }

    /// <summary>
    /// Reduz a dificuldade do Tipo de Tarefa.
    /// </summary>
    private void CreateTarefaDDA()
    {
        //REGRA DDA #1: a Dificuldade do Tipo de Tarefa é sempre reduzida em 1 nível enquanto for maior que 1.
        if (lastTarefaAprendizado.tipoTarefa.dificuldade > 1)
        {
            TipoTarefa newTipoTarefa = GerenciadorTarefas.Instance.
                GetTipoTarefa(lastTarefaAprendizado.tipoTarefa.dificuldade - 1);

            GerenciadorTarefas.Instance.Log(string.Format("\tReducao da Dificuldade do Tipo de Tarefa: {0} -> {1}",
                lastTarefaAprendizado.tipoTarefa, newTipoTarefa));

            lastTarefaAprendizado.tipoTarefa = newTipoTarefa;
        }

        //REGRA DDA #2: Reduzir o número de comparações da tarefa com base no numero de regressoes realizadas.
        if (lastTarefaAprendizado.numRegressoes >= NUMERO_REGRESSOES_PARA_REDUCAO_COMPARACOES &&
            lastTarefaAprendizado.comparacoes.Length > 2)
        {
            ReduzirComparacoes();
        }

    }

    private void ReduzirComparacoes()
    {
        lastTarefaAprendizado.RegredirNumEscolhas();
        GerenciadorTarefas.Instance.Log("\tReducao do Numero de Comparacoes.");
    }

    private void SaveTarefaOnBuffer()
    {
        bufferTarefasPendentes.Push((TarefaAprendizado) lastTarefaAprendizado.Clone());
        GerenciadorTarefas.Instance.Log("\tTarefa " + lastTarefaAprendizado.tipoTarefa.nome + 
            " salva no buffer");
    }

    private bool IsPossivelRegressao()
    {
        return lastTarefaAprendizado.tipoTarefa.dificuldade > 1f;
    }
}
