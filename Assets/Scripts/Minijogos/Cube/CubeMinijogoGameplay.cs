using UnityEngine;

namespace Minijogos
{
    public class CubeMinijogoGameplay : MinijogoGameplay
    {
        //private Vector3 leftAnswerPosition;
        //private Vector3 rightAnswerPosition;
        //private Vector3 centerAnswerPosition;
        //private float answerLength = 0f;

        protected override void SetCentralPosition()
        {
        	centralLookPosition = transform.position + Vector3.up * GetCubeHeigth();
        }

        //protected override void OnUpdate()
        //{
        //    DebugDraw.DrawVector(centralLookPosition, Vector3.right, range.x / 2f, 2f, Color.blue, 0f);
        //    DebugDraw.DrawVector(centralLookPosition, Vector3.left, range.x / 2f, 2f, Color.blue, 0f);
            
        //    DebugDraw.DrawVector(leftAnswerPosition, Vector3.right, answerLength, 2f, Color.red, 0f);
        //    DebugDraw.DrawMarker(leftAnswerPosition, 2f, Color.red, 0f);
        //    DebugDraw.DrawMarker(centerAnswerPosition, 2f, Color.red, 0f);
        //}

        protected override void PlaceConfirmations()
        {
            negativeConfirmationInstance.transform.position = centralLookPosition +
                Vector3.left * (range.x - negativeConfirmationInstance.Width) / 2f;

            positiveConfirmationInstance.transform.position = centralLookPosition +
                Vector3.right * (range.x - positiveConfirmationInstance.Width) / 2f;
        }        

        protected override void PlaceChoices()
        {
            Vector3 lastPosition = negativeConfirmationInstance.transform.position +
                Vector3.right * (negativeConfirmationInstance.Width / 2f + choiceDistance.x + choiceInstance.Width / 2f);

            for (int i = 0; i < tarefa.NumTentativas; i++)
            {
                choices[i].transform.position = lastPosition;
                lastPosition += Vector3.right * (choices[i].Width + choiceDistance.x);
            }            
        }

        protected override void SetAnswerPositions()
        {
            answersPosition = new Vector3[choices.Length];
            Vector3 upVector = Vector3.up * (choiceInstance.Height + choiceDistance.y);

            for (int i = 0; i < answersPosition.Length; i++)
            {
                answersPosition[i] = choices[i].transform.position + upVector;
                choices[i].Index = i;
            }

            //centerAnswerPosition = centralLookPosition + upVector;
            //leftAnswerPosition = FirstChoice.transform.position + Vector3.left * FirstChoice.Width / 2f + upVector;
            //rightAnswerPosition = LastChoice.transform.position + Vector3.right * LastChoice.Width / 2f + upVector;

            //answerLength = Vector3.Distance(rightAnswerPosition, leftAnswerPosition);
        }

        protected override void SetWidthRange(){
            range.x = (negativeConfirmationInstance.Width + choiceDistance.x) +
                       tarefa.NumTentativas * (choiceInstance.Width + choiceDistance.x) +
                       positiveConfirmationInstance.Width;
        }

        protected override void SetHeightRange()
        {
            range.y = GetCubeHeigth() + 5f;
        }

        private float GetCubeHeigth()
        {
            return Amaru.Instance.GetHeight +
                choiceDistance.y + negativeConfirmationInstance.Height;
        }

        protected override float GetBackwardDistance()
        {
            return range.x * 0.5f;
        }
    }

}

