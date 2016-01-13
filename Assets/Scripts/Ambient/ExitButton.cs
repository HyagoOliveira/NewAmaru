using Minijogos;
using UnityEngine;

public class ExitButton : MonoBehaviour {

    void Start()
    {
        this.gameObject.SetActive(false);
    }

	void OnMouseDown()
    {
        GerenciadorTarefas.Instance.ExitGame();
    }
}
