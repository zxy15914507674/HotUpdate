
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


//该脚本不需要拖拽到场景中的游戏物体上

public class LoadResource : MonoBehaviour {

	
	void Start () {
	    	
	}
	
	void Update () {
        
	}


    /// <summary>
    /// 下载预制体并进行动态加载预制体
    /// </summary>
    /// <param name="FileName">已经打包后的预制体所在的文件名(如果有后缀，加上后缀)</param>
    /// <param name="PrefabName">预制体的名称</param>
    /// <param name="position">预制体生成的位置</param>
    /// <param name="ScriptNames">预制体上挂载的脚本，如果没有，则该参数不用设置</param>
    public void InitializePrefabs(string FileName, string PrefabName, Vector3 position, params MonoBehaviour[] ScriptNames)
    { 
          if(FileName==null||PrefabName==null||
             FileName.Length==0||PrefabName.Length==0){
                 return;
          }
          if (ScriptNames!= null && ScriptNames.Length > 0)
          {
              StartCoroutine(DownLoadPrefab(FileName, PrefabName,position, ScriptNames));
          }
          else {
              StartCoroutine(DownLoadPrefab(FileName, PrefabName,position));
          }
          
    }

    /// <summary>
    /// 下载预制体并进行加载
    /// </summary>
    /// <param name="FileName">已经打包后的预制体所在的文件名(如果有后缀，加上后缀)</param>
    /// <param name="PrefabName">预制体的名称</param>
    /// <param name="ScriptNames">预制体上挂载的脚本,如果没有，则该参数不用设置</param>
    public void InitializePrefabs(string FileName, string PrefabName, params MonoBehaviour[] ScriptNames)
    {
        if (FileName == null || PrefabName == null ||
           FileName.Length == 0 || PrefabName.Length == 0)
        {
            return;
        }
        if (ScriptNames != null && ScriptNames.Length > 0)
        {
            StartCoroutine(DownLoadPrefab(FileName, PrefabName,Vector3.zero, ScriptNames));
        }
        else
        {
            StartCoroutine(DownLoadPrefab(FileName, PrefabName,Vector3.zero));
        }

    }

    private IEnumerator DownLoadPrefab(string FileName, string PrefabName, Vector3 position, params MonoBehaviour[] ScriptNames)
    {
        Scene scene = SceneManager.GetActiveScene();
        string SceneName = scene.name;
        string path = "file://" + System.Environment.CurrentDirectory + "\\SceneResources\\OtherResoruces\\" + SceneName + "\\" + FileName;
        if (!File.Exists(System.Environment.CurrentDirectory + "\\SceneResources\\OtherResoruces\\" + SceneName + "\\" + FileName))
        {
            yield break;
        }
        
        WWW www2 = new WWW(path);
        yield return www2;
        if (!string.IsNullOrEmpty(www2.error))
        {
            Debug.LogError(www2.error);
        }
        else
        {
            AssetBundle ab = www2.assetBundle;
            UnityEngine.Object obj = ab.LoadAsset(PrefabName);
            
            if(obj!=null){
                GameObject pre=obj as GameObject;
                
                if(pre!=null){
                    GameObject instancePre = Instantiate(pre);
                    instancePre.name = PrefabName;
                    if(position!=Vector3.zero){
                        instancePre.transform.position = position;
                    }
                    if(ScriptNames!=null&&ScriptNames.Length>0){
                        foreach (var item in ScriptNames)
                        {
                            instancePre.AddComponent(item.GetType());
                        }
                    }
                }
            }

            ab.Unload(false);

            
            
        }

        www2.Dispose();
    }


    /// <summary>
    /// 动态更换游戏物体的贴图
    /// </summary>
    /// <param name="gameObject">需要更换贴图的游戏物体</param>
    /// <param name="FileName">打包后的贴图的文件名</param>
    /// <param name="PictureName">贴图的名称</param>
    public void ChangeGameObjectTextures(GameObject gameObject, string FileName, string PictureName)
    {
        if (gameObject == null || FileName == null || PictureName == null || FileName.Length == 0 || PictureName.Length == 0)
        {
            return;
        }
        StartCoroutine(DownLoadTextures(gameObject, FileName, PictureName));
    }


    private IEnumerator DownLoadTextures(GameObject gameObject,string FileName,string TextureName) {
        Scene scene = SceneManager.GetActiveScene();
        string SceneName = scene.name;
        string path = "file://" + System.Environment.CurrentDirectory + "\\SceneResources\\OtherResoruces\\" + SceneName + "\\" + FileName;
        if (!File.Exists(System.Environment.CurrentDirectory + "\\SceneResources\\OtherResoruces\\" + SceneName + "\\" + FileName))
        {
            yield break;
        }

        WWW www2 = new WWW(path);
        yield return www2;
        if (!string.IsNullOrEmpty(www2.error))
        {
            Debug.LogError(www2.error);
        }
        else
        {
            AssetBundle ab = www2.assetBundle;
            UnityEngine.Object obj = ab.LoadAsset(TextureName);
            
            if (obj != null)
            {
               if(gameObject!=null&&gameObject.GetComponent<Renderer>()!=null){
                   gameObject.GetComponent<Renderer>().material.mainTexture = obj as Texture;
               }
            }
            ab.Unload(false);


        }

        www2.Dispose();
    }



    /// <summary>
    /// 动态加载并播放音频
    /// </summary>
    /// <param name="gameObject">要挂载音频的游戏物体(需要添加AudioSource组件)</param>
    /// <param name="FileName">打包后的文件名</param>
    /// <param name="AudioName">音频的名称(包括后缀名）</param>
    public void PlayAudioClip(GameObject  gameObject, string FileName,string AudioName) { 
        if(gameObject==null||FileName==null||AudioName==null||FileName.Length==0||AudioName.Length==0){
            return;
        }
        StartCoroutine(DownLoadAudio(gameObject, FileName, AudioName));
    }



    private IEnumerator DownLoadAudio(GameObject gameObject,string FileName,string AudioName) {
        Scene scene = SceneManager.GetActiveScene();
        string SceneName = scene.name;
        string path = "file://" + System.Environment.CurrentDirectory + "\\SceneResources\\OtherResoruces\\" + SceneName + "\\" + FileName;
        if (!File.Exists(System.Environment.CurrentDirectory + "\\SceneResources\\OtherResoruces\\" + SceneName + "\\" + FileName))
        {
            yield break;
        }

        WWW www2 = new WWW(path);
        yield return www2;
        if (!string.IsNullOrEmpty(www2.error))
        {
            Debug.LogError(www2.error);
        }
        else
        {
            AssetBundle ab = www2.assetBundle;
            UnityEngine.Object obj = ab.LoadAsset(AudioName);

            AudioClip audioClip = obj as AudioClip;

            if (audioClip != null && gameObject.GetComponent<AudioSource>() != null)
            {
                AudioSource audioSource = gameObject.GetComponent<AudioSource>();
                audioSource.clip = audioClip;

                StartCoroutine(PlayAudioClip(audioSource, audioClip, ab));
            }
            else {
                ab.Unload(false);
            }

            
            



        }

        www2.Dispose();
    }



    private IEnumerator PlayAudioClip(AudioSource audioSource,AudioClip audioClip, AssetBundle assetBundle)
    {
        while (audioClip.loadState == AudioDataLoadState.Loading)
        {
            yield return null;
        }
        assetBundle.Unload(false);
        if (audioClip.loadState == AudioDataLoadState.Loaded)
        {
            audioSource.Play();
        }
    }
}
