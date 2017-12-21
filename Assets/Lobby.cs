using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Code.Objects;
using Code.Objects.Common;
using UnityEngine;
using UnityEngine.UI;


public class Lobby : MonoBehaviour
{
    public class Container<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;
    }

    public GameObject ProfileGameObject;

    public GameObject RootCanvas;

    public User SelectedProfile;

    public List<Container<User, Text>> _profiles = new List<Container<User, Text>>();
    // Use this for initialization

    public void Start()
    {
        foreach (var file in Directory.GetFiles(@"D:\profileStorage"))
            _profiles.Add(new Container<User, Text>()
            {
                Item1 = JsonUtility.FromJson<User>(File.ReadAllText(file)),
                Item2 = null
            });

        var yMax = 272.15f;
        var i = 0;
        foreach (var key in _profiles)
        {
            yMax -= i * 59.7f;

            var pos = new Vector3(-479.12f, yMax);

            var obj = Instantiate(ProfileGameObject, pos, Quaternion.identity,
                RootCanvas.transform);

            obj.transform.localPosition = pos;
            var comp = obj.GetComponent<ProfileComponent>();

            key.Item2 = obj.GetComponentInChildren<Text>();

            obj.GetComponentInChildren<Text>().text = key.Item1.Name;

            comp.User = key.Item1;
            comp.Text = key.Item2;
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}