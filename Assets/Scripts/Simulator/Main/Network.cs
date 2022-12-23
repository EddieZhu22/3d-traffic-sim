using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace TrafficSim
{
    [Serializable]
    public class Network
    {
        public static int MAX_LABEL_COST_IN_SHORTEST_PATH = 10000;


        // the length of simulation time (min)
        public static int LENGTH_OF_SIMULATION_TIME_HORIZON_IN_MIN = 90;

        //the number of seconds per simulation interval
        public static int NUMBER_OF_SECONDS_PER_SIMU_INTERVAL = 6;

        public int LENGTH_OF_SIMULATION_TIME_HORIZON_IN_INTERVAL = LENGTH_OF_SIMULATION_TIME_HORIZON_IN_MIN * 60 / NUMBER_OF_SECONDS_PER_SIMU_INTERVAL;

        public int NUMBER_OF_SIMU_INTERVALS_PER_MIN = 60 / NUMBER_OF_SECONDS_PER_SIMU_INTERVAL;

        //start time of the simulation: 9999 as default value to be updated in reading 
        //function for agent file
        public float g_simulation_start_time_in_min = 9999;

        //end time of the simulation: 0 as default value to be updated in reading 
        //function for agent file
        public float g_simulation_end_time_in_min = 0;

        //the number of start simulation interval
        public int g_start_simu_interval_no = 0;

        //the number of end simualation interval 
        public int g_end_simu_interval_no = 0;


        // input data statistics
        public int g_number_of_nodes = 0;
        public int g_number_of_links = 0;
        public int g_number_of_agents = 0;

        // Result Statistics
        public int g_cumulative_arrival_count = 0;
        public int g_cumulative_departure_count = 0;

        public List<Building> poi_list = new List<Building>();
        public List<Node> node_list = new List<Node>();
        public List<Link> link_list = new List<Link>();
        public List<Agent> agent_list = new List<Agent>();
        public Dictionary<int, List<int>> agent_td_list_dict = new Dictionary<int, List<int>>();

        public Dictionary<int, int> node_seq_to_link_seq = new Dictionary<int, int>();
        public Dictionary<int, int> internal_node_seq_no_dict = new Dictionary<int, int>();
        public Dictionary<int, Vector3> external_node_pos = new Dictionary<int, Vector3>();
        public List<int> node_predecessor;
        public float[] node_label_cost;
        public List<int> link_predecessor;
        public int[] node_OutgoingLinkSize;
        public int[] link_volume_array;
        public List<float> link_cost_array;

        public Link[] links;

        public Node[] nodes;

        public void init()
        {
            for (int i = 0; i < link_list.Count; i++)
            {
                link_list[i].ResetMOE();
                link_list[i].CalculateBPRFunction();
            }
        }
        #region Allocation
        public void allocate()
        {
            //initialization of shortest path
            // allocate (Correct I beleive)
            node_label_cost = new float[node_list.Count];
            for (int i = 0; i < node_label_cost.Length; i++)
                node_label_cost[i] = MAX_LABEL_COST_IN_SHORTEST_PATH;
            for (int i = 0; i < node_list.Count; i++)
                link_predecessor.Add(-1);
            for (int i = 0; i < node_list.Count; i++)
                node_predecessor.Add(-1);
            for (int i = 0; i < link_list.Count; i++)
                link_cost_array.Add(-1);
            link_volume_array = new int[link_list.Count];

            node_OutgoingLinkSize = new int[node_list.Count];

            for (int i = 0; i < link_list.Count; i++)
            {
                link_cost_array[i] = link_list[i].properties.cost;
            }
            for (int i = 0; i < link_list.Count; i++)
            {
                node_OutgoingLinkSize[link_list[i].properties.from_node_seq_no] += 1;
            }
            for (int i = 0; i < node_list.Count; i++)
                node_OutgoingLinkSize[i] = 0;
            for (int i = 0; i < link_list.Count; i++)
            {
                node_OutgoingLinkSize[link_list[i].properties.from_node_seq_no] += 1;
            }
        }
        #endregion
        public Dictionary<string, dynamic> optimal_label_correcting(Dictionary<string, dynamic> return_value, int origin_node, int destination_node, float departure_time, string sp_algm = "fifo")
        {
            
            /*
            """ find shorest path between origin_node and destination_node

            input : origin_node, destination_node, departure_time
            output: the shortest path

            sp_algm: please choose one of the following three, fifo, deque, and
            dijkstra.

            The implementations are adopted from 
            https://github.com/jdlph/shortest-path-algorithms
            """
            */
            origin_node = internal_node_seq_no_dict[origin_node];
            destination_node = internal_node_seq_no_dict[destination_node];
            if (node_list[origin_node].outgoing_link_list == null)
                return null;

            // Initialization for all nodes

            // Dont think this is necessary
            for (int i = 0; i < node_label_cost.Length; i++)
                node_label_cost[i] = MAX_LABEL_COST_IN_SHORTEST_PATH;
            for (int i = 0; i < node_list.Count; i++)
                link_predecessor[i] = -1;
            for (int i = 0; i < node_list.Count; i++)
                node_predecessor[i] = -1;
            node_label_cost[origin_node] = departure_time;
            List<int> status = new int[node_list.Count].ToList();

            if (sp_algm.ToLower() == "fifo")
            {
                List<int> SEList = new List<int>();
                SEList.Add(origin_node);
                while (SEList.Count > 0)
                {
                    int from_node = SEList[0];
                    SEList.RemoveAt(0);
                    status[from_node] = 0;
                    for (int k = 0; k < node_list[from_node].outgoing_link_list.Count; k++) //correct
                    {
                        int to_node = node_list[from_node].outgoing_link_list[k].properties.to_node_seq_no; //correct
                        float new_to_node_cost = node_label_cost[from_node] + link_cost_array[node_list[from_node].outgoing_link_list[k].properties.link_seq_no]; //.correctg
                        if (new_to_node_cost < node_label_cost[to_node]) //correct
                        {
                            node_label_cost[to_node] = new_to_node_cost;//correct
                            node_predecessor[to_node] = from_node; 
                            link_predecessor[to_node] = node_list[from_node].outgoing_link_list[k].properties.link_seq_no;
                            if (status[to_node] == 0)//correct
                            {
                                SEList.Add(to_node);//correct
                                status[to_node] = 1;//correct
                            }
                        }
                    }
                }
            }
            else if (sp_algm.ToLower() == "deque")
            {
                // might break. .pop function not yet implemented. Rememebr: Pop is removing and taking
                var SEList = new List<int>();
                SEList.Add(origin_node);
                for (int j = 0; j < SEList.Count; j++)
                {
                    int from_node = SEList[SEList.Count];
                    status[from_node] = 2;
                    for (int i = 0; i < node_list[from_node].outgoing_link_list.Count; i++)
                    {
                        int to_node = node_list[from_node].outgoing_link_list[i].properties.to_node_seq_no;
                        float new_to_node_cost = node_label_cost[from_node] + link_cost_array[node_list[from_node].outgoing_link_list[i].properties.link_seq_no];

                        if (new_to_node_cost < node_label_cost[to_node])
                        {
                            node_label_cost[to_node] = new_to_node_cost;
                            node_predecessor[to_node] = from_node;
                            link_predecessor[to_node] = node_list[from_node].outgoing_link_list[i].properties.link_seq_no;
                            if (status[to_node] != 1)
                                if (status[to_node] == 2)
                                    SEList.Prepend(to_node);
                                else
                                    SEList.Add(to_node);
                            status[to_node] = 1;
                        }
                    }
                }
            }
            else if (sp_algm.ToLower() == "dijkstra")
            {
                // not a heap
                var SEList = new List<float>();
                SEList.Add(node_label_cost[origin_node]);
                SEList.Add(origin_node);
                for (int i = 0; i < SEList.Count / 2; i++)
                {
                    float label_cost = SEList[0];
                    int from_node = Convert.ToInt32(SEList[1]);
                    for (int k = 0; k < node_list[from_node].outgoing_link_list.Count; k++)
                    {
                        int to_node = node_list[from_node].outgoing_link_list[k].properties.to_node_seq_no;
                        float new_to_node_cost = label_cost + link_cost_array[node_list[from_node].outgoing_link_list[k].properties.link_seq_no];
                        if (new_to_node_cost < node_label_cost[to_node])
                        {
                            node_label_cost[to_node] = new_to_node_cost;

                            node_predecessor[to_node] = from_node;

                            link_predecessor[to_node] = node_list[from_node].outgoing_link_list[k].properties.link_seq_no;

                            SEList.Add(node_label_cost[to_node]);
                            SEList.Add(to_node);
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException("Please choose correct shortest path algorithm: " + "fifo or deque or dijkstra");
            }

            // end of fifo: 

            if (origin_node >= 0 && node_label_cost[origin_node] < MAX_LABEL_COST_IN_SHORTEST_PATH)
            {

                var dict = new Dictionary<string, dynamic>();
                dict.Add("path_flag", 1);
                dict.Add("node_label_cost", node_label_cost);
                dict.Add("node_predecessor", node_predecessor);
                dict.Add("link_predecessor", link_predecessor);
                return dict;
            }
            else
            {
                var dict = new Dictionary<string, dynamic>();
                dict.Add("path_flag", -1);
                return dict;
            }
        }
        float g_A2R_simu_interval(float abslute_time_in_simu_interval_no)
        {
            return abslute_time_in_simu_interval_no - g_start_simu_interval_no;
        }
        float g_R2A_simu_interval(float relative_time_in_simu_intetrval_no)
        {
            return relative_time_in_simu_intetrval_no + g_start_simu_interval_no;
        }
        string time_stamp_to_HHMMSS(double time_in_minutes)
        {
            int hours = Convert.ToInt32(time_in_minutes / 60);
            int minutes = Convert.ToInt32(time_in_minutes - hours * 60);
            int seconds = Convert.ToInt32((time_in_minutes - hours * 60 - minutes) * 60);
            return (hours.ToString() + minutes.ToString() + ":" + seconds.ToString());
        }
        string time_int_to_str(int time_int)
        {
            if (time_int < 10)
            {
                return "0" + time_int.ToString();
            }
            else
            {
                return time_int.ToString();
            }
        }
        public void g_find_shortest_path_for_agent()
        {
            for (int i = 0; i < agent_list.Count; i++)
            {
                if (agent_list[i].path_fixed_flag == 1)
                {
                    agent_list[i].path_node_seq_no_list = agent_list[i].path_node_seq;
                    if (agent_list[i].path_node_seq_no_list.Count > 1)
                    {
                        int init_node_index = 0;
                        for (int k = init_node_index + 1; k < agent_list[i].path_node_seq_no_list.Count; k++)
                        {
                            int key = agent_list[i].path_node_seq_no_list[init_node_index] * 10000 + agent_list[i].path_node_seq_no_list[init_node_index + 1];
                            int link_seq_no = node_seq_to_link_seq[key];
                            agent_list[i].path_link_seq_no_list.Add(link_seq_no);
                        }
                        agent_list[i].feasible_path_exist_flag = true;
                    }
                    continue;
                }
                Dictionary<string, dynamic> return_value = new Dictionary<string, dynamic>();
                return_value = optimal_label_correcting(return_value, agent_list[i].o_node_id, agent_list[i].d_node_id, agent_list[i].departure_time_in_min, "fifo");
                if (return_value["path_flag"] == -1)
                {
                    continue;
                }
                int current_node_seq_no = internal_node_seq_no_dict[agent_list[i].d_node_id];
                agent_list[i].path_cost = return_value["node_label_cost"][current_node_seq_no];

                while (current_node_seq_no >= 0)
                {
                    int current_link_seq_no = return_value["link_predecessor"][current_node_seq_no];

                    if (current_link_seq_no >= 0)
                    {
                        agent_list[i].path_link_seq_no_list.Insert(0, current_link_seq_no);
                    }
                    agent_list[i].path_node_seq_no_list.Insert(0, current_node_seq_no);
                    current_node_seq_no = return_value["node_predecessor"][current_node_seq_no];
                }
                if (agent_list[i].path_node_seq_no_list != null && agent_list[i].path_node_seq_no_list.Count > 1)
                {
                    agent_list[i].feasible_path_exist_flag = true;
                }

            }
        }
        public void g_TrafficSimulation()
        {
            int link_list_size = link_list.Count;
            for (int i = 0; i < link_list_size; i++)
            {
                link_list[i].ResetMOE();
                link_list[i].CalculateBPRFunction();
            }
            for (int i = 0; i < agent_list.Count; i++)
            {
                agent_list[i].Initialize_for_simulation();
            }
            for (int i = g_start_simu_interval_no; i < g_end_simu_interval_no; i++)
            {
                int number_of_simu_interval_per_min = 60 / NUMBER_OF_SECONDS_PER_SIMU_INTERVAL;
                if (i % number_of_simu_interval_per_min == 0)
                {
                    Debug.Log("simu time= " + (i / number_of_simu_interval_per_min).ToString() + "min, \n" + "CA=" + g_cumulative_arrival_count.ToString() + "\n, CD=" + g_cumulative_departure_count.ToString());
                }
                int relative_i = Convert.ToInt32(g_A2R_simu_interval(i));
                for (int k = 0; k < link_list_size; k++)
                {
                    Link link = link_list[k];
                    if (relative_i >= 1)
                    {
                        link_list[link.properties.link_seq_no].td_link_cumulative_departure[relative_i] = link_list[link.properties.link_seq_no].td_link_cumulative_departure[relative_i - 1];
                        link_list[link.properties.link_seq_no].td_link_cumulative_arrival[relative_i] = link_list[link.properties.link_seq_no].td_link_cumulative_arrival[relative_i - 1];
                    }

                }
                if (agent_td_list_dict.ContainsKey(i))
                {
                    for (int j = 0; j < agent_td_list_dict[i].Count; j++)
                    {
                        int agent_no = agent_td_list_dict[i][j];
                        Agent agent = agent_list[agent_no];
                        if (agent.feasible_path_exist_flag == true)
                        {
                            agent.b_generated = true;
                            int first_link_seq = agent.path_link_seq_no_list[0];
                            link_list[first_link_seq].td_link_cumulative_arrival[relative_i] += 1;
                            link_list[first_link_seq].entrance_queue.Add(agent.agent_seq_no);
                            g_cumulative_arrival_count += 1;
                        }
                    }
                }
                for (int j = 0; j < link_list_size; j++)
                {
                    Link link = link_list[j];
                    while (link.entrance_queue.Count > 0)
                    {
                        int agent_seq = link.entrance_queue[0];
                        link.entrance_queue.RemoveAt(0);
                        link.exit_queue.Add(agent_seq);

                        agent_list[agent_seq].veh_link_departure_time_in_simu_interval[agent_list[agent_seq].current_link_seq_no_in_path] =
                                                agent_list[agent_seq].veh_link_arrival_time_in_simu_interval[agent_list[agent_seq].current_link_seq_no_in_path]
                                                + link.properties.general_travel_time_in_simu_interval;
                    }
                }
                for (int j = 0; j < node_list.Count; j++)
                {
                    Node node = node_list[j];
                    int incoming_link_list_size = node.incoming_link_list.Count;
                    for (int k = 0; k < incoming_link_list_size; k++)
                    {
                        int incoming_link_index = (k + i) % (incoming_link_list_size);
                        int link_seq_no = node.incoming_link_list[incoming_link_index].properties.link_seq_no;
                        while (link_list[link_seq_no].td_link_out_flow_capacity[relative_i] >= 1 && link_list[link_seq_no].exit_queue.Count >= 1)
                        {
                            int agent_no = node.incoming_link_list[incoming_link_index].exit_queue[0];

                            if (agent_list[agent_no].veh_link_departure_time_in_simu_interval[agent_list[agent_no].current_link_seq_no_in_path] > i)
                            {
                                break;
                            }

                            if (agent_list[agent_no].current_link_seq_no_in_path == agent_list[agent_no].path_link_seq_no_list.Count - 1)
                            {
                                node.incoming_link_list[incoming_link_index].exit_queue.RemoveAt(0);
                                agent_list[agent_no].b_complete_trip = true;
                                link_list[link_seq_no].td_link_cumulative_departure[relative_i] += 1;
                                g_cumulative_departure_count += 1;
                            }
                            else
                            {
                                int next_link_seq = agent_list[agent_no].path_link_seq_no_list[agent_list[agent_no].current_link_seq_no_in_path + 1];
                                node.incoming_link_list[incoming_link_index].exit_queue.RemoveAt(0);
                                link_list[next_link_seq].entrance_queue.Add(agent_no);
                                agent_list[agent_no].veh_link_departure_time_in_simu_interval[agent_list[agent_no].current_link_seq_no_in_path] = i;
                                agent_list[agent_no].veh_link_arrival_time_in_simu_interval[agent_list[agent_no].current_link_seq_no_in_path + 1] = i;
                                float actual_travel_time = i - agent_list[agent_no].veh_link_arrival_time_in_simu_interval[agent_list[agent_no].current_link_seq_no_in_path];

                                float waiting_time = actual_travel_time - link_list[link_seq_no].properties.general_travel_time_in_min;
                                float temp_relative_time = g_A2R_simu_interval(agent_list[agent_no].veh_link_arrival_time_in_simu_interval[agent_list[agent_no].current_link_seq_no_in_path]);
                                int time_in_min = Convert.ToInt32(temp_relative_time / NUMBER_OF_SIMU_INTERVALS_PER_MIN);

                                link_list[link_seq_no].td_link_waiting_time[time_in_min] += waiting_time;
                                link_list[link_seq_no].td_link_cumulative_departure[relative_i] += 1;
                                link_list[next_link_seq].td_link_cumulative_arrival[relative_i] += 1;
                            }
                            agent_list[agent_no].current_link_seq_no_in_path += 1;
                            link_list[link_seq_no].td_link_out_flow_capacity[relative_i] -= 1;
                        }


                    }
                }




            }
        }
    }
}
/*
    
*/