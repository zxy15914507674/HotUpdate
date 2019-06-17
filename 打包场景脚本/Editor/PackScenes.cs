using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PackScenes : MonoBehaviour {


    [MenuItem("Custom Editor/(步骤1)打包场景")]
    static void CreateSceneALL()
    {
        //清空一下缓存  
        Caching.CleanCache();

        string path = EditorUtility.OpenFilePanel("请选择要打包的场景", "Assets", "unity");
        if (path == null || path.Length == 0)
        {
            Debug.LogError("请选择打包的场景");
            return;
        }
        if (!Directory.Exists(System.Environment.CurrentDirectory + "\\SceneResources"))
        {
            Directory.CreateDirectory(System.Environment.CurrentDirectory + "\\SceneResources");
        }

        string[] strPath = path.Split('/');
        string SceneName;
        if (strPath != null && strPath.Length > 0)
        {
            SceneName = strPath[strPath.Length - 1].Split('.')[0];
        }
        else {
            SceneName = null;
        }
       
        //获得用户选择的路径的方法，可以打开保存面板（推荐）
        string Path;
        if (SceneName != null || SceneName.Length > 0)
        {
            Path = EditorUtility.SaveFilePanel("设置场景打包后的文件名和保存路径", "SceneResources", "" + SceneName, "zxy");
        }
        else {
            Path = EditorUtility.SaveFilePanel("设置场景打包后的文件名和保存路径", "SceneResources", "" + "SceneName", "zxy");
        }
        

        if (Path == null || Path.Length == 0)
        {
            Debug.LogError("请选择保存的路径");
            return;
        }

        //另一种获得用户选择的路径，默认把打包后的文件放在Assets目录下
        //string Path = Application.dataPath + "/MyScene.unity3d";
        
        

        //选择的要保存的对象 
        string[] levels = { path};

        //打包场景  
       // BuildPipeline.BuildPlayer(levels, Path, BuildTarget.StandaloneWindows64, BuildOptions.BuildAdditionalStreamedScenes);

        BuildPipeline.BuildStreamedSceneAssetBundle(levels, Path, BuildTarget.StandaloneWindows64, BuildOptions.BuildAdditionalStreamedScenes);
        

        // 刷新，可以直接在Unity工程中看见打包后的文件
        AssetDatabase.Refresh();
    }



    [MenuItem("Custom Editor/(步骤2)打包对应场景的脚本DLL")]
    public static void BuildAssetsWindowDll()
    {

        string pathDirectory = EditorUtility.OpenFilePanel("请选择要打包的场景对应的脚本(只能选择Assets下的Dll文件夹下的文件)", "Assets", "bytes");
        Debug.Log(pathDirectory);
        if(pathDirectory==null||pathDirectory.Length==0){
            Debug.LogError("请选择打包的bytes格式的脚本文件");
            return;
        }
        if(!Directory.Exists(System.Environment.CurrentDirectory + "\\SceneResources\\Scripts")){
            Directory.CreateDirectory(System.Environment.CurrentDirectory + "\\SceneResources\\Scripts");
        }

        string []pathArray = pathDirectory.Split('/');
        string fileName=pathArray[pathArray.Length-1];
        Object mainAsset = AssetDatabase.LoadMainAssetAtPath("Assets/" + pathArray[pathArray.Length - 2] + "/" + fileName);
        BuildPipeline.BuildAssetBundle(mainAsset, null, System.Environment.CurrentDirectory + "\\SceneResources\\Scripts\\"+fileName.Split('.')[0]+".script",
                               BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);


    }


      [MenuItem("Custom Editor/(步骤3)建立场景与脚本联系的XML文件(注意要先打开场景)")]
       public static void SetXMLFile()
        {

            //保存xml文件的路径
            string path = System.Environment.CurrentDirectory + "\\SceneResources\\XML";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            Scene scene = SceneManager.GetActiveScene();
            string SceneName = scene.name;
            //选择保存的路径和名称
            string FilePath = EditorUtility.SaveFilePanel("设置XML文件保存的路径和名称(注意与场景名称一致)", "SceneResources\\XML", "" + SceneName, "xml");
            if(FilePath==null||FilePath.Length==0){
                Debug.LogError("请设置保存的文件名");
                return;
            }
            Debug.Log(FilePath);

            //查找到当前所有的带有脚本的游戏物体的MonoBehaviour
            MonoBehaviour[] scripts = Object.FindObjectsOfType<MonoBehaviour>();

            WriteXMLFile(scripts,FilePath);
        }



      /// <summary>
      /// 写入XML文件
      /// </summary>
      /// <param name="scripts">脚本集合</param>
      /// <param name="FilePath">XML文件保存的路径</param>
      private static void WriteXMLFile(MonoBehaviour[] scripts, string FilePath)
      {
          if (!File.Exists(FilePath))
          {
              FileStream stream = File.Create(FilePath);
              stream.Close();
          }


          XmlDocument xDoc = new XmlDocument();
          XmlDeclaration xmlDec = xDoc.CreateXmlDeclaration("1.0", "utf-8", "yes");
          xDoc.AppendChild(xmlDec);

          //创建根节点,并加入到xDoc中
          xDoc.AppendChild(xDoc.CreateElement("root"));

          //保存,参数是文件名
          xDoc.Save(FilePath);


          XmlDocument doc = new XmlDocument();
          doc.Load(FilePath);                 //加载文档
          XmlElement root = doc.DocumentElement;     //获取根节点
          Debug.Log("加载完毕");

          foreach (MonoBehaviour item in scripts)
          {
              if (item.ToString().Contains("("))
              {


                  string scriptNameTmp = item.ToString().Split(new char[] { '(', ')' })[1];
                  //因为直接输出item.ToString()的值为  游戏物体名称(脚本名称)
                  Debug.Log(item.gameObject.name + ":" + scriptNameTmp);

                  if (!scriptNameTmp.Contains("."))
                  {
                      XmlElement gameObject = doc.CreateElement("gameObject");//插入一个gameObject节点  


                      XmlElement gameObjectName = doc.CreateElement("gameObjectName");
                      gameObjectName.InnerText = item.gameObject.name;
                      XmlElement scriptName = doc.CreateElement("scriptName");
                      scriptName.InnerText = item.ToString().Split(new char[] { '(', ')' })[1];


                      gameObject.AppendChild(gameObjectName);
                      gameObject.AppendChild(scriptName);




                      root.AppendChild(gameObject);       //把新节点添加到根节点下  

                      doc.Save(FilePath);//保存文档
                  }
              }

          }

      }



     
     [MenuItem("Custom Editor/打包场景资源(如预制体，图片等)")]
     public static void CreateSceneResource(){
         //清空一下缓存  
         Caching.CleanCache();

         //保存资源文件文件的路径
         Scene scene = SceneManager.GetActiveScene();
         string SceneName = scene.name;
         string path = System.Environment.CurrentDirectory + "\\SceneResources\\OtherResoruces\\"+SceneName;
         if (!Directory.Exists(path))
         {
             Directory.CreateDirectory(path);
         }

         //进行场景资源的打包
         BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None,BuildTarget.StandaloneWindows);
         AssetDatabase.Refresh();
     }
    

}
