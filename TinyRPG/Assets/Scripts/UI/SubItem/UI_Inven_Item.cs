using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inven_Item : UI_Base
{
    enum GameObjects
    {
        ItemIcon,
        ItemNameText,
    }

    string _name;

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));

        Get<GameObject>((int)GameObjects.ItemNameText).gameObject.GetComponent<Text>().text = _name;
        Get<GameObject>((int)GameObjects.ItemIcon).AddUIEvent((PointerEventData data) => { Debug.Log($"아이템 클릭 {_name}"); });
    }

    void Update()
    {

    }

    public void SetInfo(string name)
    {
        _name = name;
    }
}
