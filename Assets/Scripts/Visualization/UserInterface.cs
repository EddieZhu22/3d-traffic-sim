using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TrafficSim
{
    public class UserInterface : MonoBehaviour
    {
        public GameObject Panel;
        [SerializeField] private int state;
        public void DropDownButton() // for the button that toggles 3D and 2D view
        {
            Animator panelAnim = Panel.GetComponent<Animator>();
            if(state == 0)
            {
                panelAnim.SetBool("zoom", false);
                state = 1;
            }
            else
            {
                panelAnim.SetBool("zoom", true);
                state = 0;
            }
        }
    }

}
