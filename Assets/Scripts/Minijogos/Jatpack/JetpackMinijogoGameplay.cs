using UnityEngine;

namespace Minijogos
{
    public class JetpackMinijogoGameplay : MinijogoGameplay
    {
        public GameObject jetpackPrefab;
        public GameObject rayGunPrefab;
        private RayGun rayGunInstance;
        private Jetpack jetpackInstance;


        protected override void OnBeforeOpen()
        {            
            Vector3 pos = transform.position;
            pos.x += -range.x / 2f + 4f;
            pos.y = jetpackPrefab.GetComponent<Renderer>().bounds.size.y + 0.5f;

            GameObject jInstance = Instantiate(jetpackPrefab, pos, jetpackPrefab.transform.rotation)
                as GameObject;
            jetpackInstance = jInstance.GetComponent<Jetpack>();
        }

        //protected override void OnUpdate()
        //{
        //    float distance = Vector3.Distance(answersPosition[0], answersPosition[answersPosition.Length - 1]);
        //    DebugDraw.DrawVector(answersPosition[0], Vector3.right, distance, 2f, Color.blue, 0f);
        //    DebugDraw.DrawMarker(answersPosition[0], 2f, Color.blue, 0f);

        //    DebugDraw.DrawVector(centralLookPosition, Vector3.up, range.y/2f, 2f, Color.blue, 0f);
        //    DebugDraw.DrawVector(centralLookPosition, Vector3.down, range.y/2f, 2f, Color.blue, 0f);

        //}

        public override void OnGetItemTool(GameplayToolComponent tool)
        {
            if(jetpackInstance.GetType() == tool.GetType())
            {
                jetpackInstance.SetTopPoint(range.y);
                rayGunInstance = Instantiate(rayGunPrefab).GetComponent<RayGun>();
            }
        }

        protected override void PlaceChoices()
        {
            Vector3 lastPosition = negativeConfirmationInstance.transform.position +
                Vector3.up * (negativeConfirmationInstance.Height / 2f + choiceDistance.y + choiceInstance.Height / 2f);

            for (int i = 0; i < tarefa.NumTentativas; i++)
            {
                choices[i].transform.position = lastPosition;
                lastPosition += Vector3.up * (choices[i].Height + choiceDistance.y);
            }
        }

        protected override void PlaceConfirmations()
        {
            Vector3 rightDistance = transform.position + Vector3.right * 
                (range.x / 2f - negativeConfirmationInstance.Width);

            negativeConfirmationInstance.transform.position = rightDistance +
                Vector3.up * (negativeConfirmationInstance.Height + 0.5f);

            positiveConfirmationInstance.transform.position = rightDistance +
                Vector3.up * (range.y + 0.5f);
        }

        protected override void SetCentralPosition()
        {
            centralLookPosition = transform.position + Vector3.up * (range.y / 2f + 1f);
        }

        protected override void SetAnswerPositions()
        {
            answersPosition = new Vector3[choices.Length];
            Vector3 lastPosition = positiveConfirmationInstance.transform.position +
                Vector3.left * (tarefa.NumTentativas-1) * (choiceInstance.Width + choiceDistance.x) +
                Vector3.up * (choiceInstance.Height + choiceDistance.y + positiveConfirmationInstance.Width / 2f);

            float upDistance = Mathf.Ceil(lastPosition.y - positiveConfirmationInstance.transform.position.y);
            cameraZoomOutPan = Vector3.up * upDistance;

            for (int i = 0; i < answersPosition.Length; i++)
            {
                answersPosition[i] = lastPosition;
                choices[i].Index = i;
                lastPosition += Vector3.right * (choices[i].Width + choiceDistance.x);
            }

        }

        protected override void SetHeightRange()
        {
            range.y = (negativeConfirmationInstance.Height + choiceDistance.y) +
                       tarefa.NumTentativas * (choiceInstance.Height + choiceDistance.y) +
                       positiveConfirmationInstance.Height;
        }

        protected override void SetWidthRange()
        {
            range.x = 20f;
        }

        protected override void DropTools()
        {
            rayGunInstance.Drop();
            jetpackInstance.Drop();
        }

        protected override float GetBackwardDistance()
        {
            float distance = range.y + cameraZoomOutPan.y;

            return distance > 10f ? distance : 10f;
        }
    }

}

