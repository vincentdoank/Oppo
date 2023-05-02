using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class ErrorMessageInfo
{
    public string errCode;
    public string message;
}

public class ErrorMessageTranslator : MonoBehaviour
{
    public List<ErrorMessageInfo> errMessageList;
    public static ErrorMessageTranslator Instance { get; private set; }

    public void Start()
    {
        Instance = this;        
    }

    public static string Translate(string errCode)
    {
        return Instance.errMessageList.Where(x => x.errCode == errCode).Select(x => x.message).FirstOrDefault();
    }
}
