using ALEPP;
using UnityEngine;

namespace Minijogos
{
	public class GerenciadorTarefas : MonoBehaviour, ISingletonManager
	{
        public float minijogosDistance = 25f;
        public GameObject minijogoCubo;
        public GameObject minijogoJetpack;

        public static GerenciadorTarefas Instance { get; private set; }
        public const int NUM_ALTO_TENTATIVAS = 8;
        
        private DDAAgent ddaAgent;

        public SessionData Data { get; private set; }
        public GameObject MinijogoParent { get; private set; }
        public MinijogoGameplay CurrentMinijogoGameplay { get; private set; }
        public bool IsMinijogoRodando
        {
            get
            {
                if (CurrentMinijogoGameplay == null)
                    return false;

                return CurrentMinijogoGameplay.Actived;
            }
        }

        private GameObject[] minijogosPrefabs;
        private string log = string.Empty;
        

        void Awake()
        {
            SetSingleton();    
        }

        public void SetSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                print("Gerenciador de tarefas existente. Esse sera excluido.");
                DestroyImmediate(this.gameObject);
            }

        }


        void Start()
        {
            ddaAgent = new DDAAgent(MinijogoType.CUBE, MinijogoType.JETPACK);
            Data = new SessionData();

            minijogosPrefabs = new GameObject[2];
            minijogosPrefabs[0] = minijogoCubo;
            minijogosPrefabs[1] = minijogoJetpack;
        }

        #region Publics methods
        public void StartSession(SessionData data)
        {
            this.Data = data;
            CreateMinijogoParent();
            Data.IndexTarefaAtual = -1;
            NextMinijogoGameplay();
            BackgroundMusicManager.Instance.StartPlay();
            Log("Inicio da Sessao: " + System.DateTime.Now.ToString("H:mm:ss - dd/MM/yyyy"));
        }

        public TarefaAprendizado NextTarefaAprendizado()
        {
            Log("\tBusca proxima tarefa do arquivo de repertorio.");

            Data.IndexTarefaAtual++;
            if (Data.IndexTarefaAtual < Data.TarefasAprendizado.Length)
            {
                return Data.TarefasAprendizado[Data.IndexTarefaAtual];
            }

            FinalizarSessao();
            return null;                
        }       

        public bool UltrapassaNumeroElevadoTentativas()
        {
            return Data.TarefaAprendizadoAtual.NumTentativas > NUM_ALTO_TENTATIVAS;
        }

        public void FinalizaMinijogoGamePlay()
        {
            Log("Tarefa Concluida: " + Data.TarefaAprendizadoAtual);
            ddaAgent.AtualizarRankMinijogo(Data.TarefaAprendizadoAtual);

            if (Data.TarefaAprendizadoAtual.AcertouUltimaVez)
            {
                Log("\tAcertou Tarefa.");
                ddaAgent.AtualizaConhecimentoPalavra(Data.TarefaAprendizadoAtual);

                if (ddaAgent.IsTarefaAprendizadoPendente())
                {
                    Data.TarefaAprendizadoAtual = ddaAgent.GetTarefaAprendizadoPendente();
                    CriarMinijogo();
                }
                else
                {
                    Log("\tNao ha tarefas pendentes no Buffer");
                    NextMinijogoGameplay();
                }
            }
            else
            {
                //ativa DDA!!
                Log("\tErrou Tarefa.");
                ddaAgent.Run(Data.TarefaAprendizadoAtual);
                Data.TarefaAprendizadoAtual = ddaAgent.GetTarefaAprendizado();
                CriarMinijogo();
            }

            Log(""); //uma linha pra separar uma tarefa da outra
        }       

        public void ExitGame()
        {
            Application.Quit();
        }

        public TipoTarefa GetTipoTarefa(int dificuldade)
        {
            foreach (TipoTarefa tt in Data.TiposTarefas.Values)
            {
                if (dificuldade == tt.dificuldade)
                    return tt;
            }

            throw new UnityException("Tipo de Tarefa nao encontrado com dificuldade igual a " + dificuldade);
        }
        #endregion        

        #region Tarefa e Minijogos methods
        private void NextMinijogoGameplay()
        {         
            if(Data.IndexTarefaAtual < Data.TarefasAprendizado.Length - 1)
            {
                Data.IndexTarefaAtual++;

                Vector3 lastPosition = CurrentMinijogoGameplay == null ? Vector3.zero :
                    CurrentMinijogoGameplay.transform.position;
                
                CriarMinijogo(lastPosition + Vector3.right * minijogosDistance);
            }
            else
            {
                FinalizarSessao();
            }            
        }

        private void CriarMinijogo()
        {
            Vector3 lastPosition = CurrentMinijogoGameplay.transform.position;
            lastPosition += Vector3.right * minijogosDistance;
            CriarMinijogo(lastPosition);
        }

        private void CriarMinijogo(Vector3 position)
		{
            GameObject minijogoGameplayInstance = null;

            //Minijogo é escolhido de acordo com a tarefa atual   
            switch (Data.TarefaAprendizadoAtual.tipoMinijogo)
            {
                //Caso não haja nenhum tipo de minijogo especificado no arquivo de texto, os
                //tipos de minijogos irão ficar se alternando
                case MinijogoType.NONE:
                    int index = Data.IndexTarefaAtual % minijogosPrefabs.Length;
                    minijogoGameplayInstance = Instantiate(minijogosPrefabs[index], position,
                                        minijogosPrefabs[index].transform.rotation) as GameObject;
                    break;

                case MinijogoType.CUBE:
                    minijogoGameplayInstance = Instantiate(minijogoCubo, position,
                                        minijogoCubo.transform.rotation) as GameObject;
                    break;
                case MinijogoType.JETPACK:
                    minijogoGameplayInstance = Instantiate(minijogoJetpack, position,
                                        minijogoJetpack.transform.rotation) as GameObject;
                    break;
            }

            CurrentMinijogoGameplay = minijogoGameplayInstance.GetComponent<MinijogoGameplay>();
            CurrentMinijogoGameplay.AttachTarefa(Data.TarefaAprendizadoAtual);
        }

		private void FinalizarSessao()
		{
            Log("Fim da Sessao: " + System.DateTime.Now.ToString("H:mm:ss - dd/MM/yyyy"));
            PrintLog();
            //TODO: Ir para a tela de finaliazação do game
            BackgroundMusicManager.Instance.FadeOut();
            Fader.Instance.FadeInOut(null, null);
		}

        //Cria um gameobject varia para ser pai de todos os Minijogos
        private void CreateMinijogoParent()
        {
            string TAG = "MinijogosParent";
            MinijogoParent = GameObject.FindGameObjectWithTag(TAG);
            if (MinijogoParent == null)
            {
                MinijogoParent = new GameObject(TAG);
                MinijogoParent.tag = TAG;
            }
        }
        #endregion

        #region DEBUG

        public void Log(string message)
        {
            log += message + "\n";
            print(message);
        }

        private void PrintLog()
        {
            if (!System.IO.Directory.Exists(Paths.SESSION_DIR))
            {
                System.IO.Directory.CreateDirectory(Paths.SESSION_DIR);
            }
            string filename = string.Format("Sessao_{0}.txt", System.DateTime.Now.ToString("Hmmss"));
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(Paths.SESSION_DIR + filename))
            {
                file.Write(log);
            }
        }

        private void gerarRelatorioFinal()
        {
            string text = "Tipo Tarefa;Peso;Dificuldade;Modelo;Conhecimento;Tempo Latencia;Tempo Movimento;"+
                "Tempo Minijogo;Tempo Parado;Acertou Ultima Vez;Dificuldade Estatica;Dificuldade Dinamica;Escolhas";

            foreach (TarefaAprendizado ta in Data.TarefasAprendizado)
            {
                ddaAgent.Run(ta);
                text += string.Format("\n{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12}",
                    ta.tipoTarefa.nome, ta.tipoTarefa.peso, ta.tipoTarefa.dificuldade, ta.modelo.nome, 
                    ta.modelo.conhecimentoGeral, ta.TempoLatencia, ta.tempoMovimento, ta.tempoMinijogo, 
                    ta.tempoParado, ta.AcertouUltimaVez, ta.dificuldadeEstatica, ta.dificuldadeDinamica, ta.Comparacoes);
            }

            text = text.Replace('.', ',');

            using (System.IO.StreamWriter file = 
                new System.IO.StreamWriter(@"C:\Users\Hyago\Desktop\RelatorioFinal.csv"))
            {
                file.Write(text);
            }
        }
        #endregion
    }
}

