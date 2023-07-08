using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;

public class ItemEditor : EditorWindow
{
    private ItemSourceDataSO database;
    private List<ItemDetail> item_list=new ();//map with ItemDataList_SO
    private VisualTreeAsset item_row_T;
    private ListView list_view;
    private ScrollView detail_view;
    private ItemDetail active_item;
    private VisualElement icon_big;
    private EnumField enum_view;

    [MenuItem("Custom/ItemEditor")]
    public static void ShowExample()
    {
        ItemEditor wnd = GetWindow<ItemEditor>();
        wnd.titleContent = new GUIContent("ItemEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        //VisualElement label = new Label("Hello World! From C#");
        //root.Add(label);

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/UI Builder/ItemEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        item_row_T = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/UI Builder/ItemRowTemplate.uxml");
        list_view = root.Q<VisualElement>("ItemList").Q<ListView>("ListView");
        detail_view = root.Q<ScrollView>("ItemDetail");
        icon_big = detail_view.Q<VisualElement>("Icon_big");
        enum_view = detail_view.Q<EnumField>("Type");
        enum_view.Init(ItemType.None);
        root.Q<Button>("AddButton").clicked += OnAddButtonClicked;
        root.Q<Button>("DeleteButton").clicked += OnDeleteButtonClicked;
        detail_view.visible = false;
        BindDataBase();
        GenerateListView();
    }

    private void OnDeleteButtonClicked()
    {
        item_list.Remove(active_item);
        list_view.Rebuild();
        detail_view.visible = false;
    }

    private void OnAddButtonClicked()
    {
        ItemDetail new_item = new();
        new_item.item_name = "未命名的新物体";
        new_item.id=1001+item_list.Count;
        item_list.Add(new_item);
        list_view.selectedIndex=item_list.Count-1;
        list_view.Rebuild();
    }

    private void BindDataBase()
    {
        string[] guids = AssetDatabase.FindAssets($"t:{nameof(ItemSourceDataSO)}");
        if(guids.Length > 0 )
        {
            string path =AssetDatabase.GUIDToAssetPath(guids[0]);
            database=AssetDatabase.LoadAssetAtPath<ItemSourceDataSO>(path);
            item_list = database.item_details;
            EditorUtility.SetDirty(database);
        }
    }

    private void GenerateListView()
    {

        Func<VisualElement> make_item = () =>
        {
            return item_row_T.CloneTree();
        };
        Action<VisualElement, int> bind_item = (e, i) =>
        {
            if (i < item_list.Count)
            {
                if (item_list[i].icon != null)
                    e.Q<VisualElement>("Icon").style.backgroundImage = item_list[i].icon ?
                                                                    item_list[i].icon.texture : null;
                e.Q<Label>("Name").text = item_list[i].item_name;
                e.Q<Label>("ID").text = item_list[i].id.ToString();
            }
        };
        list_view.itemsSource = item_list;
        list_view.makeItem = make_item;
        list_view.bindItem = bind_item;
        list_view.Sort(CompareItemRow);
        list_view.selectionChanged += OnSelectItemChange;
    }

    private int CompareItemRow(VisualElement x, VisualElement y)
    {
        Debug.Log("Comparing...");
        int x_id=int.Parse(x.Q<Label>("ID").text);
        int y_id=int.Parse(y.Q<Label>("ID").text);
        return x_id - y_id;
    }

    private void OnSelectItemChange(IEnumerable<object> select)
    {
        active_item = select.First() as ItemDetail;
        GetItemDetail();
        detail_view.visible = true;
    }

    private void GetItemDetail()
    {
        detail_view.MarkDirtyRepaint();
        detail_view.Q<IntegerField>("ID").value = active_item.id;
        detail_view.Q<IntegerField>("ID").RegisterValueChangedCallback(evt => 
            active_item.id = evt.newValue);


        detail_view.Q<TextField>("Name").value = active_item.item_name;
        detail_view.Q<TextField>("Name").RegisterValueChangedCallback(evt =>
        {
            active_item.item_name = evt.newValue;
            list_view.Rebuild();
        });

        detail_view.Q<EnumField>("Type").value = active_item.item_type;
        detail_view.Q<EnumField>("Type").RegisterValueChangedCallback(evt =>
            active_item.item_type = (ItemType)evt.newValue);

        detail_view.Q<EnumField>("Type").value = active_item.item_type;
        detail_view.Q<EnumField>("Type").RegisterValueChangedCallback(evt =>
            active_item.item_type = (ItemType)evt.newValue);

        detail_view.Q<ObjectField>("Icon").value = active_item.icon;
        //FIX 为什么在register之前的valuechange也可以响应
        icon_big.style.backgroundImage = active_item.icon?  active_item.icon.texture : null;
        detail_view.Q<ObjectField>("Icon").RegisterValueChangedCallback(evt =>
        {
            Sprite new_icon= evt.newValue as Sprite;
            active_item.icon = new_icon;
            //flush the view
            icon_big.style.backgroundImage = new_icon ? new_icon.texture : null;
            list_view.Rebuild();
        });

        detail_view.Q<ObjectField>("WorldSprite").value = active_item.world_sprite;
        detail_view.Q<ObjectField>("WorldSprite").RegisterValueChangedCallback(evt =>
            active_item.world_sprite = evt.newValue as Sprite);

        detail_view.Q<TextField>("Description").value = active_item.description;
        detail_view.Q<TextField>("Description").RegisterValueChangedCallback(evt =>
            active_item.description = evt.newValue);

        detail_view.Q<IntegerField>("UseRadius").value = active_item.use_radius;
        detail_view.Q<IntegerField>("UseRadius").RegisterValueChangedCallback(evt =>
            active_item.use_radius = evt.newValue);

        detail_view.Q<Toggle>("CanPick").value = active_item.can_pick;
        detail_view.Q<Toggle>("CanPick").RegisterValueChangedCallback(evt =>
            active_item.can_pick = evt.newValue);

        detail_view.Q<Toggle>("CanDrop").value = active_item.can_drop;
        detail_view.Q<Toggle>("CanDrop").RegisterValueChangedCallback(evt =>
            active_item.can_drop = evt.newValue);

        detail_view.Q<Toggle>("CanCarry").value = active_item.can_carry;
        detail_view.Q<Toggle>("CanCarry").RegisterValueChangedCallback(evt =>
            active_item.can_carry = evt.newValue);

        detail_view.Q<IntegerField>("Price").value = active_item.price;
        detail_view.Q<IntegerField>("Price").RegisterValueChangedCallback(evt =>
            active_item.price = evt.newValue);

        detail_view.Q<Slider>("SellCount").value = active_item.sell_count;
        detail_view.Q<Slider>("SellCount").RegisterValueChangedCallback(evt =>
            active_item.sell_count = evt.newValue);
    }
}