using System;

namespace ALEPP
{
	public class Usuario : EntidadePersistivel
	{
		public DateTime inicioUltimaSessao;
		public DateTime finalUltimaSessao;
		public float tempoMedioMinijogo;


		public Usuario() : base(0, "sem nome"){}

		public Usuario(int id, string nome) : base(id, nome){}
	}
}