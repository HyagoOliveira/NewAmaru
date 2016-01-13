using Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ALEPP
{
    public class TarefaAprendizado : ICloneable
    {
        public TipoTarefa tipoTarefa;
        public Palavra modelo;
        public Palavra[] comparacoes;
        public MinijogoType tipoMinijogo = MinijogoType.NONE;      
        public float dificuldadeEstatica;
        public float dificuldadeDinamica;
        public float tempoMinijogo;
        public float tempoMovimento;
        public float tempoParado;
        public float tempoInicioMinijogo;
        public int numCorrecoes;
        public int numRegressoes;

        public bool Concluida { get; private set; }

        public int NumTentativas { 
            get 
            {
                if (tipoTarefa.GetFormaExibicaoAlternativas().Equals((char)DefinicaoTipoTarefa.SILABAS))
                {
                    int numSilabasComparacoes = 0;
                    for (int i = 0; i < comparacoes.Length; i++)
                    {
                        numSilabasComparacoes += comparacoes[i].numSilabas;
                    }
                    return numSilabasComparacoes;
                }

                return comparacoes.Length; 
            } 
        }

        public float TempoLatencia { get { return tempoMinijogo - tempoMovimento; } }

        public bool AcertouUltimaVez { get; private set; }

        public TarefaAprendizado(string tipoTarefa, string modelo, string[] comparacoes)
        {
            this.tipoTarefa = GameAssetsLoader.Instance.Data.GetTipoTarefa(tipoTarefa);
            this.modelo = GameAssetsLoader.Instance.Data.GetPalavra(modelo);
            this.comparacoes = new Palavra[comparacoes.Length];
            for (int i = 0; i < this.comparacoes.Length; i++)
            {
                this.comparacoes[i] = GameAssetsLoader.Instance.Data.GetPalavra(comparacoes[i]);
            }

            dificuldadeEstatica = dificuldadeDinamica = 0f;
            numRegressoes = numCorrecoes = 0;
            tempoMovimento = tempoMinijogo = tempoParado = 0f;
            Concluida = false;
        }

        public bool IsFormaExibicaoPalavra()
        {
            return tipoTarefa.IsExibicaoPalavra();
        }

        public bool IsFormaExibicaoSilabica()
        {
            return tipoTarefa.IsExibicaoSilabica();
        }

        public string Comparacoes
        {
            get
            {
                string line = comparacoes[0].nome;
                for (int i = 1; i < comparacoes.Length; i++)
                {
                    line += ", " + comparacoes[i].nome;
                }

                return line;
            }
        }
        

        public void RegredirNumEscolhas()
        {
            List<Palavra> _comparacoes = new List<Palavra>(comparacoes);
            _comparacoes.RemoveAt(_comparacoes.Count - 1);
            this.comparacoes = _comparacoes.ToArray();
        }

        public void FinalizarTarefa(string palavraEscolhida)
        {            
            AcertouUltimaVez = modelo.nome.Equals(palavraEscolhida);
            Concluida = true;
        }

        public string[] GetSilabasOrdenadas()
        {
            List<string> silabas = new List<string>();
            foreach (Palavra p in comparacoes)
            {
                for (int i = 0; i < p.silabas.Length; i++)
                {
                    silabas.Add(p.silabas[i]);
                }
            }
            
            return silabas.ToArray();
        }

        public string[] GetSilabasRandom()
        {
            System.Random rnd = new System.Random();
            return GetSilabasOrdenadas().OrderBy(item => rnd.Next()).ToArray();
        }

        public Palavra[] GetComparacoesRandom()
        {
            System.Random rnd = new System.Random();
            return comparacoes.OrderBy(item => rnd.Next()).ToArray();
        }

        public override string ToString()
        {
            return string.Format("Tipo Tarefa -> [{0}]; Modelo -> [{1}]; TL: {2}; TMov: {3}, TMj: {4}, TP: {5}, Acertou Ultima: {6}",
                tipoTarefa, modelo, TempoLatencia, tempoMovimento, 
                tempoMinijogo, tempoParado, AcertouUltimaVez);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class TarefaAprendizadoContainer
    {
        public TarefaAprendizado[] TarefasAprendizado;

        public TarefaAprendizadoContainer() { }

        public static TarefaAprendizadoContainer Load(string filePath, string fileName)
        {
            TarefaAprendizadoContainer container = new TarefaAprendizadoContainer();

            string[] lines = DataSerializator.LoadTextFile(filePath, fileName, "csv");
            if (lines == null)
                throw new UnityException("Arquivo de Repertorio não encontrado!");

            container.TarefasAprendizado = new TarefaAprendizado[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                string[] lineComponents = lines[i].Split(';');
                int firstComparacaoIndex = 1;
                for (int k = 0; k < firstComparacaoIndex; k++)
                {
                    if (string.IsNullOrEmpty(lineComponents[k]))
                        throw new UnityException("Tipo de Tarefa, Modelo ou Primeira Comparação nulo. Necessário haver no minimo tres campos.");
                }

                List<string> comparacoes = new List<string>();

                for (int k = firstComparacaoIndex; k < lineComponents.Length; k++)
                {
                    if (!string.IsNullOrEmpty(lineComponents[k]))
                        comparacoes.Add(lineComponents[k]);
                    else
                        break;
                }

                container.TarefasAprendizado[i] = new TarefaAprendizado(lineComponents[0], lineComponents[1], comparacoes.ToArray());
            }

            return container;
        }
    }    
}

