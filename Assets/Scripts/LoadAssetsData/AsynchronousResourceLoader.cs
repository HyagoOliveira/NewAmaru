using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AsynchronousData
{
	public class AsynchronousResourceLoader : MonoBehaviour
	{
        public delegate void SuccessCallbackEvent<T>(T[] response);
        public delegate void FailCallbackEvent(UnityException exception);


        public void RequestAllTexturesFromFolder(string folderPath,
            SuccessCallbackEvent<Texture2D> OnSuccess, FailCallbackEvent OnFail)
		{	
			DirectoryInfo dInfo = new DirectoryInfo(folderPath);
            if (!dInfo.Exists){
                OnFail(new UnityException(string.Format("Diretório {0} não encontrado!", folderPath)));
                return;
            }

            StartCoroutine(LoadTexturesCouroutine(dInfo, OnSuccess, OnFail));
		}

        public void RequestAllAudioClipsFromFolder(string folderPath,
            SuccessCallbackEvent<AudioClip> OnSuccess, FailCallbackEvent OnFail)
        {
            DirectoryInfo dInfo = new DirectoryInfo(folderPath);
            if (!dInfo.Exists)
            {
                OnFail(new UnityException(string.Format("Diretório {0} não encontrado!", folderPath)));
                return;
            }

            StartCoroutine(LoadAudioClipCouroutine(dInfo, OnSuccess, OnFail));
        }


        private IEnumerator LoadTexturesCouroutine(DirectoryInfo dInfo,
            SuccessCallbackEvent<Texture2D> OnSuccess, FailCallbackEvent OnFail) 
        {
        	List<Texture2D> textures = new List<Texture2D>();
        	//ler todas as imagens da pasta
            foreach (FileInfo fi in dInfo.GetFiles("*.png", SearchOption.AllDirectories))
            {                
                WWW www = new WWW("file:///" + fi.FullName); // Start a download of the given URL
                yield return www; // Wait for download to complete
                //print("Imagem carregada em: " + fi.FullName);
                Texture2D texture = www.texture;
                texture.name = fi.Name.Split('.')[0];
                textures.Add(texture);
            }       

            if(textures.Count == 0)
            {
                OnFail(new UnityException("Nenhuma imagem encontrada na pasta."));
            }

			OnSuccess(textures.ToArray());
	    }

        private IEnumerator LoadAudioClipCouroutine(DirectoryInfo dInfo,
            SuccessCallbackEvent<AudioClip> OnSuccess, FailCallbackEvent OnFail)
        {
            List<AudioClip> audios = new List<AudioClip>();
            //ler todas as imagens da pasta
            foreach (FileInfo fi in dInfo.GetFiles("*.ogg", SearchOption.AllDirectories))
            {
                WWW www = new WWW("file:///" + fi.FullName); // Start a download of the given URL
                yield return www; // Wait for download to complete
                //print("Imagem carregada em: " + fi.FullName);
                AudioClip audio = www.GetAudioClip(false, true, AudioType.OGGVORBIS);
                audio.name = fi.Name.Split('.')[0];
                audios.Add(audio);
            }

            if (audios.Count == 0)
            {
                OnFail(new UnityException("Nenhum audio clip encontrado na pasta."));
            }

            OnSuccess(audios.ToArray());
        }
	}
}