using Item;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFunction : MonoBehaviour
{
    public ItemPackSO pack_data;
    private bool bag_is_open;
    // Start is called before the first frame update
    void Start()
    {
        OpenBag();
        if(bag_is_open && Input.GetKeyDown(KeyCode.Escape))
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenBag()
    {
        bag_is_open = true;
        PackUIManager.instance.OpenOtherBag(pack_data.slot_datas);

    }
}
