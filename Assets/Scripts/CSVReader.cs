using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

/// <summary>
/// Reads CSV then spits out parsed data. 
/// Can import any GMNS data format, will support other formats soon
/// </summary>
public class CSVReader : MonoBehaviour
{

    public string NodeFileName, LinkFileName;

    List<Node> nodeList = new List<Node>();
    List<Link> linkList = new List<Link>();

    public GameObject nodePrefab;
    public GameObject linkPrefab;

    public int multiplier;

    private Vector3 linkPos;

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
                        node.id = Convert.ToInt32(node.data[i]);
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
                nodeList.Add(node);
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
                        Vector3 pos = Vector3.zero;
                        FindNodePosition(Convert.ToInt32(link.data[i]));
                        pos = linkPos;
                        LinkObj.GetComponent<LineRenderer>().SetPosition(0, pos);
                    }
                    if (link.names[i] == "to_node_id")
                    {
                        Vector3 pos = Vector3.zero;
                        FindNodePosition(Convert.ToInt32(link.data[i]));
                        pos = linkPos;
                        Debug.Log(Convert.ToInt32(link.data[i]));
                        LinkObj.GetComponent<LineRenderer>().SetPosition(1, pos);
                    }
                }
                //Debug.Log(offset);

                num++;
            }


        }
    }
    private void FindNodePosition(int id)
    {
        for(int i = 0; i < nodeList.Count; i++)
        {
            if(nodeList[i].id == id)
            {
                linkPos = nodeList[i].transform.position;
                Debug.Log(nodeList[i].transform.position);
            }
        }
    }
}
