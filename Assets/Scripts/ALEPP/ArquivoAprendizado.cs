namespace ALEPP
{
	public class ArquivoAprendizado : EntidadePersistivel
	{
		public string path;

        public ArquivoAprendizado(int id, string nome, string path) 
            : base(id, nome)
        {
            this.path = path;
        }
	}

}

