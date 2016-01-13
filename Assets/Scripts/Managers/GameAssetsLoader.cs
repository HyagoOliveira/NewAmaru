using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using AsynchronousData;
using ALEPP;
using System;
using System.Xml;
using Serialization;
using Minijogos;
using System.IO;

[RequireComponent(typeof(AsynchronousResourceLoader))]
public class GameAssetsLoader : MonoBehaviour, ISingletonManager
{
    public Text statusText;
    public Text logText;
    public float textReadTime = 2f;
    public ExitButton exitButton;

    private AsynchronousResourceLoader loader;
    private float timeToRead;
    public SessionData Data { get; private set; }

    public static GameAssetsLoader Instance { get; private set; }

    private bool IN_LOADING_SCENE;
    private const string GAME_CONTROLLER_TAG = "GameController";

    void Awake()
    {
        SetSingleton();
    }

    public void SetSingleton()
    {
        if (IsDataLoaded() || Instance != null)
        {
            DestroyImmediate(this);
            return;
        }

        Instance = this;
    }



    // Use this for initialization
    void Start()
    { 
        loader = GetComponent<AsynchronousResourceLoader>();
        Data = new SessionData();
        IN_LOADING_SCENE = Application.loadedLevelName == "LOADING_SCENE"; // SceneManager.GetActiveScene().name == "LOADING_SCENE";
        if (IN_LOADING_SCENE)
        {
            statusText.enabled = logText.enabled = false;
            timeToRead = textReadTime;
            Fader.Instance.FadeOut(StartLoadData);
        }
        else
        {
            timeToRead = 0f;
            Fader.Instance.FadeOut();
            StartLoadData();
        }
    }

    public void ShutDown()
    {
        BackgroundMusicManager.Instance.FindAudioSource();
        GerenciadorTarefas.Instance.StartSession(Data);
        Destroy(this);
        Destroy(loader);
    }

    private void StartLoadData()
    {
        LoadDataAssets();
        Invoke("LoadResourcesAssets", timeToRead);
    }

    private void LoadDataAssets()
    {
        showLoadMessage("Carregando arquivos de texto...");

        try
        {
            LoadTipoTarefas();
            LoadPalavras();
            LoadRepertorio();
        }
        catch (UnityException exception)
        {
            showLogError("Erro ao converter arquivo:\n\n" + exception.Message);
        }
        catch (XmlException exception)
        {
            showLogError("Erro ao converter arquivo XML:\n\n" + exception.Message);
        }        
        catch (DirectoryNotFoundException)
        {
            showLogError("Diretório de Arquivos não encontrado:\n\n" +
                "O diretório deve estar em " + Paths.DATA_DIR);
        }
        catch (Exception exception)
        {
            showLogError("Erro ao converter arquivo:\n\n" + exception.Message);
        }
        
    }

    private void LoadTipoTarefas()
    {
        TipoTarefaContainer ttContainer = TipoTarefaContainer.Load(Paths.SETTINGS_DIR, "TipoTarefas");
        if (ttContainer == null || ttContainer.TipoTarefas == null || ttContainer.TipoTarefas.Length == 0)
        {
            throw new UnityException("Arquivo de Tipo de Tarefas vazio em " + Paths.SETTINGS_DIR + "TipoTarefas");
        }
        Data.SetTipoTarefas(ttContainer);
    }

    private void LoadPalavras()
    {
        PalavrasContainer pContainer = PalavrasContainer.Load(Paths.SETTINGS_DIR, "Palavras");
        if (pContainer == null || pContainer.Palavras == null || pContainer.Palavras.Length == 0)
        {
            throw new UnityException("Arquivo de Palavras vazio em " + Paths.SETTINGS_DIR + "Palavras");
        }
        Data.SetPalavras(pContainer);
    }

    private void LoadRepertorio()
    {
        TarefaAprendizadoContainer taContainer = TarefaAprendizadoContainer.Load(Paths.SETTINGS_DIR, "Repertorio");
        if (taContainer == null || taContainer.TarefasAprendizado == null || taContainer.TarefasAprendizado.Length == 0)
        {
            throw new UnityException("Arquivo de Repertorio vazio em " + Paths.SETTINGS_DIR + "Repertorio");
        }
        Data.SetRepertorio(taContainer);
    }

    private void LoadResourcesAssets()
    {
        showLoadMessage("Carregando arquivos de imagem e audio...");

        try
        {
            LoadDataImageResources();
            LoadDataAudioResources();
            LoadProximidadePalavras();
        }
        catch(UnityException exception)
        {
            showLogError("Erro ao caregar arquivo de imagem/audio:\n\n" + exception.Message);            
        }
        catch (Exception exception)
        {
            showLogError("Erro desconhecido ao caregar arquivo de imagem/audio:\n\n" + exception.Message);
        }

        Invoke("CheckLoadedData", timeToRead);
    }

    private void LoadDataImageResources()
    {
        loader.RequestAllTexturesFromFolder(Paths.IMAGES_DIR,
            Data.SetImagensEnsino, Fail);
    }

    private void LoadDataAudioResources()
    {
        loader.RequestAllAudioClipsFromFolder(Paths.AUDIO_DIR,
            Data.SetAudiosEnsino, Fail);
    }

    private void Fail(UnityException exception)
    {
        throw exception;
    }    

    private void LoadProximidadePalavras()
    {
        string[] tabela = DataSerializator.LoadTextFile(Paths.SETTINGS_DIR, "TabelaProximidades", "txt");

        if (tabela == null || tabela.Length == 0)
            throw new UnityException("Tabela de Proximidades está vazia!");

        Data.SetTabelaProximidade(tabela);
    }


    private void CheckLoadedData()
    {
        StartCoroutine(CheckLoadDataCoroutine());
    }

    private IEnumerator CheckLoadDataCoroutine()
    {
        while (!Data.isDataFullyLoaded())
        {
            yield return new WaitForSeconds(0.1f);
        }

        showLoadMessage("Dados totalmente carregados.");

        if (!Data.isDataConsistent())
        {
            showLogError("Dados carregados estão inconsistentes!\n\n" +
                Data.getDataInconsistency());
        }

        if (IN_LOADING_SCENE)
        {
            Fader.Instance.ChangeScene("SESSAO_SCENE", ShutDown);
        }
        else
        {
            ShutDown();
        }
    }

    private void showLoadMessage(string message)
    {
        print(message);
        if(statusText != null)
        {
            statusText.enabled = true;
            statusText.text = message;
        }
    }

    private void showLogError(string errorMsg)
    {
        CancelInvoke();
        if (exitButton != null)
            exitButton.gameObject.SetActive(true);

        if (statusText != null && logText != null)
        {
            logText.text = errorMsg;
            statusText.enabled = false;
            logText.enabled = true;
        }

        throw new UnityException(errorMsg);
    }

    public bool IsDataLoaded()
    {
        return Instance != null && Instance.Data != null && Instance.Data.isDataFullyLoaded();
    }

    
}
