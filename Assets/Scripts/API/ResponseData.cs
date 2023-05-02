using System;

[Serializable]
public class ResponseData
{
    public string is_error;
    public string message;
    public ResponseGameData[] data;
}

[Serializable]
public class ResponseGameData
{
    public string cd;
    public string striker_name;
    public string goal_keeper_name;
    public string player_name_background_texture;
    public string score_container_texture;
    public string goal_status_off;
    public string goal_status_on;
    public string ball_texture;
    public string banner_texture;
    public string bench_color;
    public string field_texture_diffuse;
    public string field_texture_normal;
    public string field_texture_2_diffuse;
    public string field_texture_2_normal;
    public string texture_striker;
    public string texture_keeper;
}

[Serializable]
public class ResponseTokenData
{
    public string is_error;
    public string message;
    public TokenData data;
}

[Serializable]
public class TokenData
{
    public string Token;
}

[Serializable]
public class ImageFileData
{
    public string cd;
    public string name;
}
