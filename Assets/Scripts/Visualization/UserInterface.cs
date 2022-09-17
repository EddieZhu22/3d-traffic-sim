using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TrafficSim
{
    public class UserInterface : MonoBehaviour
    {
        public GameObject Panel;
        public SimulationEditor SimulationManager;
        [SerializeField] private int state;
        [SerializeField] private Slider mainSlider;

        public void Start()
        {
            mainSlider.onValueChanged.AddListener(delegate { ChangeResolution(); });
        }
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
        public void ChangeResolution()
        {
            SimulationManager.PosMultiplier = mainSlider.value;
            for (int i = 0; i < SimulationManager.network.node_list.Count; i++)
            {
                SimulationManager.SetNodeAttributes(SimulationManager.Node_Obj_List[i], i);
            }
            for (int i = 0; i < SimulationManager.network.link_list.Count; i++)
            {
                SimulationManager.SetLinkAttributes(SimulationManager.Link_Comp_List[i], SimulationManager.Link_Obj_List[i], i, true);
            }
        }
    }

}
