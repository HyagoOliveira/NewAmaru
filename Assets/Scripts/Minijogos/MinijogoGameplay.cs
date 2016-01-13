using ALEPP;
using System.Collections.Generic;
using UnityEngine;

namespace Minijogos
{
    [RequireComponent(typeof(SphereCollider))]
    public abstract class MinijogoGameplay : MonoBehaviour, IPauseableEntity
    {
        public Confirmation negativeConfirmationInstance;
        public Confirmation positiveConfirmationInstance;
        public Choice choiceInstance;
        public Vector2 choiceDistance = Vector2.zero;

        public bool Actived { get; protected set; }

        protected Vector2 range = Vector2.zero;
        protected Choice[] choices;
        protected Stack<int> choicesIndexStack = new Stack<int>();
        protected Vector3 centralLookPosition;
        protected Vector3[] answersPosition;
        protected TarefaAprendizado tarefa;
        protected SphereCollider fieldCollider;
        protected Vector2 cameraZoomOutPan = Vector2.zero;

        public Choice FirstChoice { get { return choices[0]; } set { choices[0] = value; } }
        public Choice LastChoice { get { return choices[choices.Length - 1]; } set { choices[choices.Length - 1] = value; } }

        protected abstract void SetWidthRange();
        protected abstract void SetHeightRange();
        protected abstract void SetCentralPosition();
        protected abstract void PlaceConfirmations();
        protected abstract void PlaceChoices();
        protected abstract void SetAnswerPositions();
        protected abstract float GetBackwardDistance();        

        protected virtual void Start()
        {
            Initialize();
            OnBeforeOpen();
            SetupComponents();
        }

        private void Initialize()
        {
            Actived = false;
            fieldCollider = GetComponent<SphereCollider>();
            transform.parent = GerenciadorTarefas.Instance.MinijogoParent.transform;
            CheckInstances();

            SetWidthRange();
            SetHeightRange();

            fieldCollider.isTrigger = true;
            fieldCollider.radius = range.x / 2f;
        }

        private void Update()
        {
            if (Actived)
            {
                tarefa.tempoMinijogo += Time.deltaTime;
                if (Amaru.Instance.input.IsMoving)
                {
                    tarefa.tempoMovimento += Time.deltaTime;
                }
                else
                {
                    tarefa.tempoParado += Time.deltaTime;
                }

                OnUpdate();
            }
        }

        public void AttachTarefa(TarefaAprendizado tarefa)
        {
            this.tarefa = tarefa;
        }

        public void SetupComponents()
        {
            SetCentralPosition();
            PlaceConfirmations();
            InstanciateChoices();
            PlaceChoices();
            SetAnswerPositions();
            RenderChoicesModel();
            ParentComponents();
            OnOpen();

            tarefa.tempoParado = tarefa.tempoMinijogo = tarefa.tempoMovimento = 0f;            
        }

        public void Finish()
        {
            OnPause();
            ResetCamera();
            OnClose();
            DropTools();
            ReturnTarefaFeedback();
            DestroyMinijogoGameplay();
        }

        public bool IsModelSelected()
        {
            return choicesIndexStack.Count != 0;
        }

        public virtual void OnGetItemTool(GameplayToolComponent tool)
        {
        }

        protected virtual void OnBeforeOpen() { }

        protected virtual void OnOpen()
        {
            GerenciadorTarefas.Instance.Log("Tarefa Criada: " + 
                GerenciadorTarefas.Instance.Data.TarefaAprendizadoAtual);
        }

        protected virtual void OnClose() { }

        protected virtual void OnUpdate() { }

        protected virtual void ReturnTarefaFeedback()
        {
            if (!IsModelSelected())
            {
                throw new UnityException("Nenhum modelo selecionado");
            }

            int[] selectedChoicesIndex = choicesIndexStack.ToArray();
            string palavraSelecionada = string.Empty;
            //palavra está invertidade devido a pilha de indexs (choicesIndexStack)
            for (int i = selectedChoicesIndex.Length - 1; i > -1 ; i--)
            {
                palavraSelecionada += choices[selectedChoicesIndex[i]].Model;
            }
            tarefa.FinalizarTarefa(palavraSelecionada);
            GerenciadorTarefas.Instance.FinalizaMinijogoGamePlay();
        }

        protected virtual void InstanciateChoices()
        {
            choices = new Choice[tarefa.NumTentativas];
            choices[0] = choiceInstance;

            for (int i = 1; i < tarefa.NumTentativas; i++)
            {
                GameObject cubeInstance = GameObject.Instantiate(choiceInstance.gameObject, 
                    choiceInstance.transform.position,
                    choiceInstance.transform.rotation) as GameObject;

                choices[i] = cubeInstance.GetComponent<SimpleChoice>();
                choices[i].Index = i - 1;
            }
        }

        protected virtual void DestroyMinijogoGameplay()
        {
            Destroy(this.gameObject);
        }

        public virtual void ChoiceSelectionAction(int index)
        {
            if (!tarefa.tipoTarefa.IsExibicaoSilabica())
            {
                NegativeSelectionAction();
            }

            choicesIndexStack.Push(index);
            choices[index].SetSelectedPosition(answersPosition[choicesIndexStack.Count - 1]);
        }

        public virtual void NegativeSelectionAction()
        {
            if (!IsModelSelected()) return;

            choices[choicesIndexStack.Pop()].ResetPosition();
        }

        public virtual void PositiveSelectionAction()
        {
            if (!IsModelSelected()) return;

            Urama.Instance.GoFollowAmaru();
            Finish();
        }

        protected virtual void ResetCamera()
        {
            Follow2DCamera.Instance.Reset();
        }

        protected virtual void LockCameraPosition()
        {
            float backwardDistance = GetBackwardDistance();
            float cameraBackwardDistance = Mathf.Abs(Follow2DCamera.Instance.transform.position.z);
            //print(backwardDistance);

            if (backwardDistance < cameraBackwardDistance)
                backwardDistance = cameraBackwardDistance;

            Follow2DCamera.Instance.LockPosition(new Vector3(centralLookPosition.x + cameraZoomOutPan.x, 
                centralLookPosition.y + cameraZoomOutPan.y, -backwardDistance));

            Vector3 leftBoundaryPos = centralLookPosition - Vector3.right * (range.x / 2f + 1f);
            Vector3 rightBoundaryPos = centralLookPosition + Vector3.right * (range.x / 2f + 1f);

            GroundController.Instance.PlaceBoundaryColliders(leftBoundaryPos, rightBoundaryPos);
        }

        protected virtual void AmaruOnField()
        {
            LockCameraPosition();
            PlaceUrama();
            OnResume();
            tarefa.tempoInicioMinijogo = Time.time;
        }

        protected virtual void PlaceUrama()
        {
            Urama.Instance.GoToInstructionPlace();
            Urama.Instance.ShowModel(tarefa);
        }

        protected virtual void SetChoicesActivation(bool active)
        {
            for (int i = 0; i < choices.Length; i++)
            {
                choices[i].gameObject.SetActive(active);
            }

            negativeConfirmationInstance.gameObject.SetActive(active);
            positiveConfirmationInstance.gameObject.SetActive(active);
        }

        protected virtual void ParentComponents()
        {
            for (int i = 0; i < choices.Length; i++)
            {
                choices[i].transform.parent = transform;
            }

            negativeConfirmationInstance.transform.parent = transform;
            positiveConfirmationInstance.transform.parent = transform;
        }

        protected virtual void RenderChoicesModel()
        {
            Palavra[] comparacoes = tarefa.GetComparacoesRandom();

            switch (tarefa.tipoTarefa.GetFormaExibicaoAlternativas())
            {
                case (char)DefinicaoTipoTarefa.IMAGEM:
                    for (int i = 0; i < choices.Length; i++)
                    {
                        RenderImageChoice(choices[i], comparacoes[i].GetImagem(), 
                            comparacoes[i].nome);
                    }
                    break;

                case (char)DefinicaoTipoTarefa.PALAVRA:
                    for (int i = 0; i < choices.Length; i++)
                    {
                        RenderTextChoice(choices[i], comparacoes[i].nome);
                    }
                    break;

                case (char)DefinicaoTipoTarefa.SILABAS:
                    string[] silabas = tarefa.GetSilabasRandom();
                    //o numero de escolhas é o mesmo numero de sílabas
                    for (int i = 0; i < silabas.Length; i++)
                    {
                        RenderTextChoice(choices[i], silabas[i]);
                    }
                    break;

                default:
                    break;
            }
        }

        protected virtual void DropTools()
        {

        }

        protected virtual void RenderImageChoice(Choice choice, Sprite sprite, string name)
        {
            choice.SetModelImage(sprite, name);
        }

        protected virtual void RenderTextChoice(Choice choice, string text)
        {
            choice.SetModelText(text);
        }

        protected virtual void CheckInstances()
        {
            if (transform.parent == null)
                throw new UnityException("Gameplay de minijogo não está dentro do game object Minijogo!");
            if (negativeConfirmationInstance == null || positiveConfirmationInstance == null)
                throw new UnityException("Instancias de confirmação estão nulas!");
            if (choiceInstance == null)
                throw new UnityException("Instancia de escolha de modelo está nula!");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag.Equals("Player"))
            {
                AmaruOnField();
                fieldCollider.enabled = false;
            }
        }

        private int[] ShuffleArray(int[] array)
        {
            for (int i = array.Length; i > 0; i--)
            {
                int j = Random.Range(0, i);
                int k = array[j];
                array[j] = array[i - 1];
                array[i - 1] = k;
            }
            return array;
        }

        public void OnPause()
        {
            Actived = false;
        }

        public void OnResume()
        {
            Actived = true;
        }
    }    
}

