using Minijogos;
using Serialization;
using UnityEngine;

namespace ALEPP
{
	public class Palavra : EntidadePersistivel
	{
		public string[] silabas;

		public int numSilabas;

		public float dificuldadeMaxLeitura;

		public float dificuldadeMaxEscrita;

		public float aprendizadoLeitura;

		public float aprendizadoEscrita;

		public float grauAprendizagemLeitura;

		public float grauAprendizagemEscrita;

		public float conhecimentoGeral;

        public bool isAprendida = false;

        public Palavra() : base(0, "sem nome"){}


        public Palavra(int id, string nome, string[] silabas)
            : base(id, nome)
        {
        	this.silabas = silabas;
        	numSilabas = this.silabas.Length;
        	dificuldadeMaxLeitura = 0f;
        	dificuldadeMaxEscrita = 0f;
        	aprendizadoLeitura = 0f;
        	aprendizadoEscrita = 0f;
        	grauAprendizagemLeitura = 0f;
        	grauAprendizagemEscrita = 0f;
        	conhecimentoGeral = 0f;
        	isAprendida = false;
        }

        public Sprite GetImagem()
        {
            return GerenciadorTarefas.Instance.Data.GetImageEnsino(id);
        }

        public AudioClip GetAudio()
        {
            return GerenciadorTarefas.Instance.Data.GetAudioEnsino(id);
        }

        public override string ToString()
        {
            return base.ToString() + string.Format(", Num Silabas: {0}", numSilabas); 
        }

        public float GetProximidade(string palavra)
        {
            return GerenciadorTarefas.Instance.Data.GetProximidadePalavra(this.nome, palavra);
        }

        public float GetProximidade(Palavra palavra)
        {
            return GetProximidade(palavra.nome);
        }
    }


	public class PalavrasContainer
	{
		public Palavra[] Palavras {get; set;}

		public PalavrasContainer(){}
		public PalavrasContainer(Palavra[] palavras){this.Palavras = palavras;}

        public static PalavrasContainer Load(string filePath, string fileName)
        {
            return DataSerializator.LoadXML<PalavrasContainer>(filePath, fileName);
        }
	}

}

