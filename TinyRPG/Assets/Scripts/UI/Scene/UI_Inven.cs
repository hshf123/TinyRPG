using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Inven : UI_Scene
{
    enum GameObjects
    {
        GridPannel
    }

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));

        GameObject gridPannel = Get<GameObject>((int)GameObjects.GridPannel);
        foreach (Transform child in gridPannel.transform)
            Managers.Resource.Destroy(child.gameObject);

        for (int i = 0; i < 9; i++)
        {
            GameObject item = Managers.UI.MakeSubItem<UI_Inven_Item>(parent: gridPannel.transform).gameObject;
            item.transform.SetParent(gridPannel.transform);

            item.GetOrAddComponent<UI_Inven_Item>().SetInfo($"БэЗа°Л{i}");
        }
    }

    void Update()
    {

    }
}
