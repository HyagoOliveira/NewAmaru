using Serialization;

namespace ALEPP
{
	public class TipoTarefa : EntidadePersistivel
	{
        public float peso;
		public int dificuldade;

        public TipoTarefa() : base() { }

        public TipoTarefa(int id, string nome, float peso, int dificuldade)
            : base(id, nome)
        {
            this.peso = peso;
            this.dificuldade = dificuldade;
        }

        public override string ToString()
        {
            return base.ToString() + string.Format(", Peso: {0}, Dificuldade: {1}", peso, dificuldade);
        }

        public char GetFormaExibicaoModelo()
        {
            return nome[0];
        }

        public char GetFormaExibicaoAlternativas()
        {
            return nome[1];
        }

        public bool IsExibicaoPalavra()
        {
            return GetFormaExibicaoAlternativas() == (char)DefinicaoTipoTarefa.PALAVRA;
        }

        public bool IsExibicaoSilabica()
        {
            return GetFormaExibicaoAlternativas() == (char)DefinicaoTipoTarefa.SILABAS;
        }
    }

    public class TipoTarefaContainer
    {
        public TipoTarefa[] TipoTarefas { get; set; }

        public TipoTarefaContainer() { }
        public TipoTarefaContainer(TipoTarefa[] tipoTarefas){ this.TipoTarefas = TipoTarefas;}

        public static TipoTarefaContainer Load(string filePath, string fileName)
        {
            return DataSerializator.LoadXML<TipoTarefaContainer>(filePath, fileName);
        }
    }

    public enum DefinicaoTipoTarefa { SOM = 'A', IMAGEM = 'B', PALAVRA = 'C', SILABAS = 'E' };
}



