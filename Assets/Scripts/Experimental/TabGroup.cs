using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{

    public float widthPadding;
    public TabSizer[] tabs;
    public int activeTab;

    void Start () {
        OnValidate();
    }

    void OnValidate() {
        for (int i = 0; i < tabs.Length; i ++) {
            tabs[i].transform.SetParent (transform);

            //set the positions of the tabs
            if (i == 0) {
                tabs[i].transform.localPosition = Vector3.zero;
            }else {
                tabs[i].transform.localPosition = new Vector3 (tabs[i-1].transform.localPosition.x + tabs[i-1].width + widthPadding, 0,0);
            }

            //set tab active or not
            if (activeTab == i)  {
                tabs[i].SetActive(true);
            }
            else  {
                tabs[i].SetActive(false);
                tabs[i].transform.localPosition = new Vector3 (
                    tabs[i].transform.localPosition.x, 
                    tabs[i].transform.localPosition.y, 
                    .1f
                );
            }
        }
    }
}
