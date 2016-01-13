using ALEPP;
using System.Collections.Generic;
using UnityEngine;

public class SessionData {
    public Dictionary<int, Sprite> ImagensEnsino { get; private set; }
    public Dictionary<int, AudioClip> AudiosEnsino { get; private set; }
    public Dictionary<string, TipoTarefa> TiposTarefas { get; private set; }
    public Dictionary<string, Palavra> Palavras { get; private set; }
    public Dictionary<string, float> ProximidadePalavras { get; private set; }
    public TarefaAprendizado[] TarefasAprendizado { get; private set; }
    //TODO: dados usuário

    public int IndexTarefaAtual = 0;

    public TarefaAprendizado TarefaAprendizadoAtual
    {
        get { return TarefasAprendizado[IndexTarefaAtual]; }
        set { TarefasAprendizado[IndexTarefaAtual] = value; }
    }

    public SessionData()
    {
    }

    #region Gets
    public Sprite GetImageEnsino(int idPalavra)
    {
        if (!ImagensEnsino.ContainsKey(idPalavra))
            throw new UnityException(string.Format("Imagem de ensino com id: {0} não encontrada!", idPalavra));

        return ImagensEnsino[idPalavra];
    }

    public AudioClip GetAudioEnsino(int idPalavra)
    {
        if (!AudiosEnsino.ContainsKey(idPalavra))
            throw new UnityException(string.Format("Audio de ensino com id: {0} não encontrado!", idPalavra));

        return AudiosEnsino[idPalavra];
    }

    public Palavra GetPalavra(string palavra)
    {
        if (!Palavras.ContainsKey(palavra))
            throw new UnityException(string.Format("Palavra: {0} nao encontrada!", palavra));

        return Palavras[palavra];
    }

    public TipoTarefa GetTipoTarefa(string tipoTarefa)
    {
        if (!TiposTarefas.ContainsKey(tipoTarefa))
            throw new UnityException(string.Format("Tipo de Tarefa: {0} nao encontrada!", tipoTarefa));

        return TiposTarefas[tipoTarefa];
    }

    public float[] GetDificuldadePalavrasConsecultivas(string palavra)
    {
        List<float> dificuldade = new List<float>();

        foreach (TarefaAprendizado t in TarefasAprendizado)
        {
            if (t.Concluida && t.modelo.nome.Equals(palavra))
                dificuldade.Add(t.dificuldadeDinamica);
        }

        return dificuldade.ToArray();
    }

    public float GetProximidadePalavra(string palavra1, string palavra2)
    {
        string key = palavra1 + "x" + palavra2;
        if (!ProximidadePalavras.ContainsKey(key))
        {
            string key2 = palavra2 + "x" + palavra1;
            if (ProximidadePalavras.ContainsKey(key2))
                return ProximidadePalavras[key];
            else
                throw new UnityException(string.Format("Proximidade: {0} ou {1} nao encontradas!", key, key2));
        }

        return ProximidadePalavras[key];
    }
    #endregion
    #region Data Assets methods
    public void SetTipoTarefas(TipoTarefaContainer ttContainer)
    {
        TiposTarefas = new Dictionary<string, TipoTarefa>();
        foreach (TipoTarefa tt in ttContainer.TipoTarefas)
        {
            TiposTarefas.Add(tt.nome, tt);
        }
    }

    public void SetPalavras(PalavrasContainer pContainer)
    {
        Palavras = new Dictionary<string, Palavra>();
        foreach (Palavra p in pContainer.Palavras)
        {
            Palavras.Add(p.nome, p);
        }
    }

    public void SetRepertorio(TarefaAprendizadoContainer taContainer)
    {
        TarefasAprendizado = taContainer.TarefasAprendizado;
    }

    public void SetImagensEnsino(Texture2D[] textures)
    {
        ImagensEnsino = new Dictionary<int, Sprite>();

        for (int i = 0; i < textures.Length; i++)
        {
            Sprite sprite = Sprite.Create(textures[i], new Rect(0f, 0f, textures[i].width, textures[i].height),
                Vector2.one * 0.5f);
            ImagensEnsino.Add(GetPalavra(textures[i].name).id, sprite);
        }
    }

    public void SetAudiosEnsino(AudioClip[] audios)
    {
        AudiosEnsino = new Dictionary<int, AudioClip>();
        for (int i = 0; i < audios.Length; i++)
        {
            AudiosEnsino.Add(GetPalavra(audios[i].name).id, audios[i]);
        }
    }

    public void SetTabelaProximidade(string[] tabela)
    {
        ProximidadePalavras = new Dictionary<string, float>();

        //primeira linha é o cabeçalho
        string[] header = tabela[0].Split(';');
        int headerIndex = 0;

        for (int line = 1; line < tabela.Length; line++)
        {
            string[] proximidades = tabela[line].Split(';');
            for (int column = 0; column < proximidades.Length; column++)
            {
                string key = header[headerIndex] + "x" + header[column];
                if (!ProximidadePalavras.ContainsKey(key))
                    ProximidadePalavras.Add(key, float.Parse(proximidades[column]));
            }
            headerIndex++;
        }
    }

    public bool isDataFullyLoaded()
    {
        return ImagensEnsino != null && AudiosEnsino != null && TiposTarefas != null && Palavras != null &&
            ProximidadePalavras != null && TarefasAprendizado != null &&
            ImagensEnsino.Count > 0 && AudiosEnsino.Count > 0 &&
            TiposTarefas.Count > 0 && Palavras.Count > 0 &&
            ProximidadePalavras.Count > 0 && TarefasAprendizado.Length > 0;
    }

    public bool isDataConsistent()
    {
        return Palavras.Count == ImagensEnsino.Count && Palavras.Count == AudiosEnsino.Count &&
            ProximidadePalavras.Count == Palavras.Count * Palavras.Count;
    }

    public string getDataInconsistency()
    {
        string incosistency = string.Empty;
        if (Palavras.Count != ImagensEnsino.Count)
            incosistency = "Numero de Palavras difere do numero de imagens de ensino.";
        if (Palavras.Count != AudiosEnsino.Count)
        {
            if (incosistency.Length > 0) incosistency += "\n";
            incosistency += "Numero de Palavras difere do numero de audios de ensino.";
        }
        if (Palavras.Count != Palavras.Count * Palavras.Count)
        {
            if (incosistency.Length > 0) incosistency += "\n";
            incosistency += "Numero de Proximidades difere do numero de palavras cadastradas.";
        }

        return incosistency;
    }
    #endregion




}
