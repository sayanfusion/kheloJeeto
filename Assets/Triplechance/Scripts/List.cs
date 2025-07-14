using System;
using System.Collections.Generic;

[System.Serializable]
public class HistoryDataListItem
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
public class GameHistory
{
    public int status;
    public List<HistoryDataListItem> list = new();
    public int total_bet;
    public int total_win;
}