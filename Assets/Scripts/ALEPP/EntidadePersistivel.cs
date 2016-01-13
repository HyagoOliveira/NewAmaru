using System.Xml;

namespace ALEPP
{
	public abstract class EntidadePersistivel
	{
        public int id;
		public string nome;

        public EntidadePersistivel() { }

        public EntidadePersistivel(int id, string nome)
        {
            this.id = id;
            this.nome = nome;
        }

        public override string ToString()
        {
            return nome;
        }
    }

}

