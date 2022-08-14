using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
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

    //end time of the simulation:0 as default value to be updated in reading 
    //function for agent file
    public int g_simulation_end_time_in_min = 0;

    //the number of start simualation interval
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


    //
    public Link[] links;

    public Node[] nodes;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
