using System;
using System.Collections.Generic;
[System.Serializable]
public class HistoryDataListItemJeeto
{
    public int id;
    public string player_id;
    public string game_id;
    public string win_number { get; set; }
    public string win_card;
    public string win_ammount;
    public string game_name;
    public string bet_ammount;
    public string win_loose;
    public string start_point;
    public object end_point;
    public string win_position;
    public object bonus_spin;
    public object ticket_id;
    public object claim_status;
    public object draw_time;
    public DateTime created_at;
    public DateTime updated_at;
}
[System.Serializable]
public class GameHistoryJeeto
{
    public int status { get; set; }
    public HistoryDataList list { get; set; }
}

public class HistoryDataList
{
    public int current_page { get; set; }
    public List<HistoryDataListItemJeeto> data { get; set; }
    public string first_page_url {get; set;}
    // public int from {get; set;}
    // public int last_page {get; set;}
    public string last_page_url { get; set; }
    public string next_page_url { get; set; }
    // public string path { get; set; }
    // public int per_page { get; set; }
    public string prev_page_url { get; set; }
    // public string to { get; set; }
    // public string total { get; set; }

}
//pagination data history
/*[System.Serializable]
public class GameHistory12
{
    public int status;
    public PaginationList list;
}

[System.Serializable]
public class PaginationList
{
    public int current_page;
    public List<GameHistoryData> data;
    public string prev_page_url;
    public string next_page_url;
}

[System.Serializable]
public class GameHistoryData
{
    public int id;
    public string game_name;
    public string win_number;
    public string win_ammount;
    public string bet_ammount;
    public string win_loose;
    public string created_at;
}*/


