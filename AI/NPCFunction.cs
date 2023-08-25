using Item;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFunction : MonoBehaviour
{
    public ItemPackSO pack_data;
    private DialogueController dia_controller;
    private bool bag_is_open;
    // Start is called before the first frame update
    void Start()
    {
        if(dia_controller == null) 
            dia_controller = GetComponent<DialogueController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bag_is_open && Input.GetKeyDown(KeyCode.Escape))
        {
            PackUIManager.instance.CloseOtherBag();
        }
    }
    
    public void CloseBag()
    {
        bag_is_open = false;
        dia_controller.is_talking = false;
        PackDataManager.instance.other_bag = null;
        PackUIManager.CloseOtherBagEvent -= CloseBag;

    }

    public void OpenBag()
    {
        bag_is_open = true;
        dia_controller.is_talking = true;
        PackDataManager.instance.other_bag = pack_data;
        PackUIManager.CloseOtherBagEvent += CloseBag;
        PackUIManager.instance.OpenOtherBag(pack_data);

    }
}
