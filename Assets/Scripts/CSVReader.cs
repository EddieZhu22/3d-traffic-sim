using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

/// <summary>
/// WIP
/// CLEAN UP CODE!
/// Reads CSV then spits out parsed data. 
/// Can import any GMNS data format, will support other formats soon
/// </summary>
public class CSVReader : MonoBehaviour
{

    public string NodeFileName, LinkFileName;

    List<Node> node_list = new List<Node>();
    List<Link> link_list = new List<Link>();

    public GameObject nodePrefab;
    public GameObject linkPrefab;

    public int multiplier;

    private Vector3 linkPos;

    [SerializeField] SimulationManager manager;

    private void Start()
    {
        CreateNodeCSV();
        CreateLinkCSV();
    }

    /// <summary>
    /// Creates a Node Using CSV Parser
    /// </summary>
    public void CreateNodeCSV()
    {
        var lines = File.ReadAllLines(Application.dataPath + "/" + NodeFileName + ".csv");
        int num = 0;
        string[] names = new string[25];
        Vector3 offset = new Vector3();
        foreach(var line in lines)
        {
            var values = line.Split(',');

            if (line == lines[0])
            {
                names = values;
            }
            if (line != lines[0])
            {

                GameObject nodeObj = Instantiate(nodePrefab);
                Node node = nodeObj.GetComponent<Node>();
                for (int i = 0; i < values.Length; i++)
                {
                    node.data = values;
                    //node.vals = new string[num + 1][];
                    //node.vals[num] = new string[values.Length];
                    //node.vals[num][i] = values[i];
                    //Debug.Log(node.vals[num][i]);
                }


                // set any relevant information.
                node.names = names;

                for (int i = 0; i < names.Length; i++)
                {
                    if(node.names[i] == "node_id")
                    {
                        node.external_node_id = Convert.ToInt32(node.data[i]);
                    }
                    if (node.names[i] == "x_coord")
                    {
                        nodeObj.transform.position = new Vector3(float.Parse(Convert.ToString(node.data[i])) * multiplier + offset.x, 0, 0);
                        node.xcoord = float.Parse(Convert.ToString(node.data[i]));
                    }
                    if (node.names[i] == "y_coord")
                    {
                        nodeObj.transform.position = new Vector3(nodeObj.transform.position.x, 0, float.Parse(Convert.ToString(node.data[i])) * multiplier + offset.z);
                        node.ycoord = float.Parse(Convert.ToString(node.data[i]));
                    }
                }
                //set origin position to zero.
                if (line == lines[1])
                {
                    offset = new Vector3(-nodeObj.transform.position.x, 0, -nodeObj.transform.position.z);
                    nodeObj.transform.position = Vector3.zero;
                }

                //Debug.Log(nodeList.Count);
                node.node_seq_no = num;

                node_list.Add(node);
                num++;
            }


        }
    }
    /// <summary>
    /// Creates A Link using CSV Parser.
    /// </summary>
    public void CreateLinkCSV()
    {
        var lines = File.ReadAllLines(Application.dataPath + "/" + LinkFileName + ".csv");
        int num = 0;
        string[] names = new string[25];
        foreach (var line in lines)
        {
            var values = line.Split(',');

            if (line == lines[0])
            {
                names = values;
            }
            if (line != lines[0])
            {

                GameObject LinkObj = Instantiate(linkPrefab);
                Link link = LinkObj.GetComponent<Link>();
                for (int i = 0; i < values.Length; i++)
                {
                    link.data = values;
                    //node.vals = new string[num + 1][];
                    //node.vals[num] = new string[values.Length];
                    //node.vals[num][i] = values[i];
                    //Debug.Log(node.vals[num][i]);
                }

                // set any relevant information.
                link.names = names;

                for (int i = 0; i < names.Length; i++)
                {
                    if (link.names[i] == "from_node_id")
                    {
                        link.external_from_node = int.Parse(Convert.ToString(link.data[i]));

                        Vector3 pos = Vector3.zero;
                        FindNodePosition(Convert.ToInt32(link.data[i]));
                        pos = linkPos;
                        LinkObj.GetComponent<LineRenderer>().SetPosition(0, pos);
                    }
                    if (link.names[i] == "to_node_id")
                    {
                        link.external_from_node = int.Parse(Convert.ToString(link.data[i]));

                        Vector3 pos = Vector3.zero;
                        FindNodePosition(Convert.ToInt32(link.data[i]));
                        pos = linkPos;
                        Debug.Log(Convert.ToInt32(link.data[i]));
                        LinkObj.GetComponent<LineRenderer>().SetPosition(1, pos);
                    }
                    if (link.names[i] == "length")
                    {
                        link.length = float.Parse(Convert.ToString(link.data[i]));
                    }
                    if (link.names[i] == "lanes")
                    {
                        link.lanes = float.Parse(Convert.ToString(link.data[i]));
                    }
                    if (link.names[i] == "free_speed")
                    {
                        link.free_speed = float.Parse(Convert.ToString(link.data[i]));
                    }
                    if (link.names[i] == "capacity")
                    {
                        link.link_capacity =                         link.free_speed = float.Parse(Convert.ToString(link.data[i])) * Convert.ToInt32(link.lanes);
                    }
                    if (link.names[i] == "link_type")
                    {
                        link.type = int.Parse(Convert.ToString(link.data[i]));
                    }
                    if (link.names[i] == "BPR_alpha1")
                    {
                        link.VDF_alpha = float.Parse(Convert.ToString(link.data[i]));
                    }
                    if (link.names[i] == "BPR_beta1")
                    {
                        link.VDF_alpha = float.Parse(Convert.ToString(link.data[i]));
                    }
                }
                //set info not related to csv file
                link.link_seq_no = num;

                // assign link's manager to manager
                link.manager = manager;
                //Debug.Log(offset);
                link_list.Add(link);
                num++;
            }


        }
    }
    private void FindNodePosition(int id)
    {
        for(int i = 0; i < node_list.Count; i++)
        {
            if(node_list[i].external_node_id == id)
            {
                linkPos = node_list[i].transform.position;
                Debug.Log(node_list[i].transform.position);
            }
        }
    }
}
