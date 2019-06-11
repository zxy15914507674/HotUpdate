using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//该脚本需要拖拽到场景中的任意一个游戏物体上进行测试
public class LoadSceneResourceTest : MonoBehaviour {

    LoadResource loadResource;
	void Start () {
        
        //******
        GameObject go = new GameObject();
        Test test=go.AddComponent<Test>();
        MonoBehaviour[] mono = { test};
        Destroy(go);
        //这部分代码是预制体本身需要添加脚本是才需要，这里是添加Test脚本
        //******




        //这段代码是测试加载预制体  MyCube和MySphere,这两个预制体都打包在名为cube的数据包内 
        loadResource=gameObject.AddComponent<LoadResource>();
        loadResource.InitializePrefabs("cube", "MyCube", mono);
        loadResource.InitializePrefabs("cube", "MySphere", new Vector3(1f, 0f, 0f), mono);
	}
	
	
	void Update () {
		if(Input.GetMouseButtonDown(0)){
            //下面的代码是测试动态添加贴图，如为名为MyCube和MySphere预制体添加名为pic的贴图
            //pic贴图打包在名为pictures的文件中
            GameObject cube = GameObject.Find("MyCube");
            GameObject sphere = GameObject.Find("MySphere");
            loadResource.ChangeGameObjectTextures(cube, "pictures", "pic");
            loadResource.ChangeGameObjectTextures(sphere, "pictures", "pic");
        }
        //下面代码是测试动态加载音频,为名为AudioSource的游戏物体添加音频剪辑并进行播放，
        //注意游戏物体AudioSource需要添加AudioSource组件，播放名为GameGUI_BackgroundAudio.mp3的音频文件
        //而该音频文件被打包在audios的文件中
        if(Input.GetMouseButtonDown(1)){
            GameObject audioGameObject = GameObject.Find("AudioSource");
            loadResource.PlayAudioClip(audioGameObject, "audios", "GameGUI_BackgroundAudio.mp3");
        }
	}
}
