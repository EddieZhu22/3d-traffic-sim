using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficSim
{
    [System.Serializable]
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
        public int g_simulation_start_time_in_min = 9999;

        //end time of the simulation: 0 as default value to be updated in reading 
        //function for agent file
        public int g_simulation_end_time_in_min = 0;

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

        public List<Node> node_list = new List<Node>();
        public List<Link> link_list = new List<Link>();
        public List<Agent> agent_list = new List<Agent>();
        public Dictionary<int, int> internal_node_seq_no_dict = new Dictionary<int, int>();
        public Dictionary<int, Vector3> external_node_pos = new Dictionary<int, Vector3>();
        public int[] node_predecessor;
        public float[] node_label_cost;
        public int[] link_predecessor;
        public int[] node_OutgoingLinkSize;
        public int[] link_volume_array;
        public int from_node_seq_no;
        public double[] link_cost_array;

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
        public void allocate()
        {
            //initialization of shortest path
            node_predecessor = new int[node_list.Count];
            node_label_cost[MAX_LABEL_COST_IN_SHORTEST_PATH] = node_list.Count;
            link_predecessor = new int[node_list.Count];
            link_volume_array = new int[link_list.Count];
            link_cost_array = new double[link_list.Count];
            node_OutgoingLinkSize = new int[node_list.Count];
            for (int i = 0; i < link_list.Count; i++)
            {
                link_cost_array[i] = link_list[i].cost;
            }
            for (int i = 0; i < link_list.Count; i++)
            {
                node_OutgoingLinkSize[link_list[i].from_node_seq_no] += 1;
            }         
        }
    }

}
/*
    def optimal_label_correcting(self, origin_node, destination_node, 
                                 departure_time, sp_algm='fifo'):
        """ find shorest path between origin_node and destination_node
        input : origin_node, destination_node, departure_time
        output: the shortest path
        sp_algm: please choose one of the following three, fifo, deque, and
        dijkstra.
        The implementations are adopted from 
        https://github.com/jdlph/shortest-path-algorithms
        """
        origin_node = self.internal_node_seq_no_dict[origin_node]
        destination_node = self.internal_node_seq_no_dict[destination_node]
        if not self.node_list[origin_node].outgoing_link_list:
            return 0
        
        # Initialization for all nodes
        self.node_label_cost = [MAX_LABEL_COST_IN_SHORTEST_PATH] * self.node_size
        # pointer to previous node index from the current label at current node
        self.node_predecessor = [-1] * self.node_size
        # pointer to previous node index from the current label at current node
        self.link_predecessor = [-1] * self.node_size
        
        self.node_label_cost[origin_node] = departure_time
        status = [0] * self.node_size

        if sp_algm.lower() == 'fifo':
            # scan eligible list
            SEList = []  
            SEList.append(origin_node)

            while SEList:
                from_node = SEList.pop(0)
                status[from_node] = 0
                for k in range(len(self.node_list[from_node].outgoing_link_list)):
                    to_node = self.node_list[from_node].outgoing_link_list[k].to_node_seq_no 
                    new_to_node_cost = self.node_label_cost[from_node] + self.link_cost_array[self.node_list[from_node].outgoing_link_list[k].link_seq_no]
                    # we only compare cost at the downstream node ToID at the new arrival time t
                    if new_to_node_cost < self.node_label_cost[to_node]:
                        # update cost label and node/time predecessor
                        self.node_label_cost[to_node] = new_to_node_cost
                        # pointer to previous physical node index from the current label at current node and time
                        self.node_predecessor[to_node] = from_node 
                        # pointer to previous physical node index from the current label at current node and time
                        self.link_predecessor[to_node] = self.node_list[from_node].outgoing_link_list[k].link_seq_no  
                        if not status[to_node]:
                            SEList.append(to_node)
                            status[to_node] = 1

        elif sp_algm.lower() == 'deque':
            SEList = collections.deque()
            SEList.append(origin_node)

            while SEList:
                from_node = SEList.popleft()
                status[from_node] = 2
                for k in range(len(self.node_list[from_node].outgoing_link_list)):
                    to_node = self.node_list[from_node].outgoing_link_list[k].to_node_seq_no 
                    new_to_node_cost = self.node_label_cost[from_node] + self.link_cost_array[self.node_list[from_node].outgoing_link_list[k].link_seq_no]
                    # we only compare cost at the downstream node ToID at the new arrival time t
                    if new_to_node_cost < self.node_label_cost[to_node]:
                        # update cost label and node/time predecessor
                        self.node_label_cost[to_node] = new_to_node_cost
                        # pointer to previous physical node index from the current label at current node and time
                        self.node_predecessor[to_node] = from_node 
                        # pointer to previous physical node index from the current label at current node and time
                        self.link_predecessor[to_node] = self.node_list[from_node].outgoing_link_list[k].link_seq_no  
                        if status[to_node] != 1:
                            if status[to_node] == 2:
                                SEList.appendleft(to_node)
                            else:
                                SEList.append(to_node)
                            status[to_node] = 1

        elif sp_algm.lower() == 'dijkstra':
            # scan eligible list
            SEList = []
            heapq.heapify(SEList)
            heapq.heappush(SEList, (self.node_label_cost[origin_node], origin_node))

            while SEList:
                (label_cost, from_node) = heapq.heappop(SEList)
                for k in range(len(self.node_list[from_node].outgoing_link_list)):
                    to_node = self.node_list[from_node].outgoing_link_list[k].to_node_seq_no 
                    new_to_node_cost = label_cost + self.link_cost_array[self.node_list[from_node].outgoing_link_list[k].link_seq_no]
                    # we only compare cost at the downstream node ToID at the new arrival time t
                    if new_to_node_cost < self.node_label_cost[to_node]:
                        # update cost label and node/time predecessor
                        self.node_label_cost[to_node] = new_to_node_cost
                        # pointer to previous physical node index from the current label at current node and time
                        self.node_predecessor[to_node] = from_node 
                        # pointer to previous physical node index from the current label at current node and time
                        self.link_predecessor[to_node] = self.node_list[from_node].outgoing_link_list[k].link_seq_no  
                        heapq.heappush(SEList, (self.node_label_cost[to_node], to_node))
        
        else:
            raise Exception('Please choose correct shortest path algorithm: '
                            +'fifo or deque or dijkstra')
        # end of sp_algm == 'fifo':
        
        if (origin_node >= 0 and self.node_label_cost[origin_node] < MAX_LABEL_COST_IN_SHORTEST_PATH):
            return {
                "path_flag": 1,
                "node_label_cost": self.node_label_cost,
                "node_predecessor": self.node_predecessor,
                "link_predecessor": self.link_predecessor
            }
        else: 
            return {"path_flag": -1}
*/