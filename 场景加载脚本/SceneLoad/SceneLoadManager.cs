using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{

    #region 场景加载不销毁
    public static SceneLoadManager instance;
    
    void Start()
    {
        //该单例脚本是避免跳转场景时出现多个该物体
        if (instance != null)
        {
            return;
        }
        else
        {
            instance = this;
            //避免场景加载时该对象销毁
            DontDestroyOnLoad(gameObject);
        }
        //新的方法 利用lambda表达式代替OnLevelWasLoaded回调方法
        SceneManager.sceneLoaded += (var, var2) =>
        {
            //根据不同场景给游戏对象变换不同位置
            if (var.buildIndex == 1)
            {
                transform.position = new Vector3(2, 3, 4);
            }
            if (var.buildIndex == 0)
            {
                transform.position = new Vector3(4, 4, 5);
            }
        };
    }
    #endregion


    AsyncOperation _asyncOperation;

    //场景名称
    string SceneName;

    AssetBundle SceneAssetBundle;
    

    void Update() {
        if (_asyncOperation!=null&&_asyncOperation.isDone)
        {
            Debug.Log("场景加载完成");
            _asyncOperation = null;
            StartCoroutine(DownLoadScriptBundle());
        }
    }





    /// <summary>
    /// 下载对应场景的脚本并动态挂载到游戏物体上
    /// </summary>
    private IEnumerator DownLoadScriptBundle()
    {
        string url = @"file://" + System.Environment.CurrentDirectory + "\\SceneResources\\Scripts\\"+SceneName+".script";

        Debug.Log("url: " + url);
        WWW www = WWW.LoadFromCacheOrDownload(url,1);

        yield return www;

        if (www.error != null)
        {
            Debug.Log("加载 出错");
            yield break;
        }

        if (www.isDone)
        {
            Debug.Log("加载完毕");
            AssetBundle ab = www.assetBundle;

            try
            {
                if(SceneName==null||SceneName.Length==0){
                    yield break;
                }

                //先把DLL以TextAsset类型取出来,在把bytes给Assembly.Load方法读取准备进入反射操作
                Assembly aly = System.Reflection.Assembly.Load(((TextAsset)www.assetBundle.mainAsset).bytes);

                //读取脚本配置信息
                Dictionary<string, string> dic = new Dictionary<string, string>();

                dic = ReadXml(System.Environment.CurrentDirectory + "\\SceneResources\\XML\\"+SceneName+".xml");

                //获取DLL下全部的类型
                foreach (var i in aly.GetTypes())
                {
                    //调试代码
                    Debug.Log(i.Name);
                    //然后类名是MyClass,就把文本引用赋值给MyClass.platefaceText属性.
                    if (i.Name != "SceneLoadManager")
                    {
                        if (dic.ContainsKey(i.Name))
                        {
                            //添加组件到当前GameObject下面
                            //Component c = this.gameObject.AddComponent(i);

                            GameObject.Find(dic[i.Name]).AddComponent(i);
                        }
                    }
                    else {
                        GameObject go =GameObject.Find("LoadSceneManager");
                        if(go!=null){
                            Destroy(go);
                        }
                        GameObject newScene = new GameObject("LoadSceneManager");
                        newScene.AddComponent(i);
                       

                    }
                    
                }

                ab.Unload(true);             //卸载assetbundle

                if(SceneAssetBundle!=null){
                    SceneAssetBundle.Unload(false);  //卸载场景assetbundle
                }
                SceneName = null;
            }
            catch (Exception e)
            {
                Debug.Log("加载DLL出错");
                Debug.Log(e.Message);
            }
        }
    
    }


    /// <summary>
    /// 读取XML文件配置信息
    /// </summary>
    /// <param name="filePath">Xml文件路径</param>
    /// <returns></returns>
    private Dictionary<string, string> ReadXml(string filePath)
    {

        Dictionary<string, string> dic = new Dictionary<string, string>();
        XDocument document = XDocument.Load(filePath);
        //获取根元素
        XElement root = document.Root;

        //获取gameObject节点下的所有元素
        IEnumerable<XElement> gameObjects = root.Elements();
        foreach (XElement gameObject in gameObjects)
        {
            string key = "";
            string value = "";
            foreach (var element in gameObject.Elements())
            {

                if (element.Name == "gameObjectName")
                {
                    value = element.Value;
                }
                else if (element.Name == "scriptName")
                {
                    key = element.Value;
                }

            }

            if (key.Length > 0 && value.Length > 0)
            {
                dic.Add(key, value);
            }
        }

        return dic;
    }




    #region 场景加载
    /// <summary>
    /// 动态加载场景
    /// </summary>
    /// <param name="SceneSourceName">场景文件的名称</param>
    /// <param name="SceneName">场景名称</param>
    /// <returns></returns>
    public void LoadScene(string SceneSourceName, string SceneName)
    {
        if (SceneSourceName == null || SceneName == null || SceneSourceName.Length == 0 || SceneName.Length == 0)
        {
            Debug.LogError("SceneLoadManager的public  IEnumerator LoadScene(string SceneSourceName,string SceneName)参数不能为null并且参数不能为空字符串");
            return;
        }
        this.SceneName = SceneName;
        StartCoroutine(LoadSceneEnumerator(SceneSourceName, SceneName));

        //StartCoroutine(LoadSceneEnumerator(SceneSourceName, SceneName));


    }


    /// <summary>
    /// 使用协程加载场景
    /// </summary>
    /// <param name="SceneSourceName">场景文件的名称</param>
    /// <param name="SceneName">场景名称</param>
    /// <returns></returns>
    private IEnumerator LoadSceneEnumerator(string SceneSourceName, string SceneName)
    {
        //强制清除缓存
        Caching.CleanCache();

        Debug.Log("file://" + System.Environment.CurrentDirectory + "\\SceneResources\\" + SceneSourceName + ".zxy");
        WWW download = WWW.LoadFromCacheOrDownload("file://" + System.Environment.CurrentDirectory + "\\SceneResources\\" + SceneSourceName + ".zxy", 1);
        yield return download;
        SceneAssetBundle = download.assetBundle;

        if (download.isDone)
        {
            //SceneManager.LoadScene(SceneName);

            _asyncOperation = SceneManager.LoadSceneAsync(SceneName);

           
        }



    }
    #endregion





    #region 获取所有场景的名称
    /// <summary>
    /// 获取所有的场景名称
    /// </summary>
    /// <returns></returns>
    public List<string> GetAllSceneName()
    {
        string path = System.Environment.CurrentDirectory + "\\SceneResources";
        string[] FileArray = Directory.GetFiles(path);
        List<string> sceneNameList = new List<string>();
        foreach (string item in FileArray)
        {
            string[] strArray = item.Split('\\');
            string sceneName = strArray[strArray.Length - 1].Replace(".zxy", "");
            Debug.Log(sceneName);
            sceneNameList.Add(sceneName);
        }
        return sceneNameList;
    }

    #endregion



}
