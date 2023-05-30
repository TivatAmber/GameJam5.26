using ExcelDataReader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Sleep : BaseObject, IReceive, IGive
{
    List<GameObject> inputs = new();
    List<GameObject> outputs = new();
    [SerializeField] List<Channel> channels = new();
    Channel outChannel;
    #region IReceive
    public bool ReceiveObject(GameObject rec)
    {
        BaseObject baseObject = rec.GetComponent<BaseObject>();

        inputs.Add(rec);
        rec.SetActive(false);
        if (outputs.Count == 0)
        {
            CheckSynthessisTable(out outputs);
        }
        return true;
    }
    #endregion
    #region IGive
    public bool GiveObject(Kind which = 0)
    {
        if (which == 0)
        {
            if (inputs.Count > 0)
            {
                GameManager.Instance.nowGameObject = inputs[0];
                GameManager.Instance.nowGameObject.SetActive(true);
                inputs.RemoveAt(0);
                return true;
            }
        }
        else
        {
            Kind tmp;
            GameObject target = null;
            for (int i = 0; i < inputs.Count; i++)
            {
                tmp = BaseOpe.GetKind(inputs[i]);
                if (tmp == which && target == null)
                {
                    target = inputs[i];
                    inputs.RemoveAt(i);
                }
            }
            if (target == null) return false;
            GameManager.Instance.nowGameObject = target;
            GameManager.Instance.nowGameObject.SetActive(true);
        }
        return true;
    }
    #endregion
    public bool OutObject()
    {
        if (outputs.Count == 0) return false;
        GameManager.Instance.nowGameObject = outputs[0];
        GameManager.Instance.nowGameObject.SetActive(true);
        outputs.RemoveAt(0);
        return true;
    }
    private bool ClearChannel()
    {
        foreach (Channel cha in channels)
        {
            if (cha is InChannel)
            {
                var now = cha as InChannel;
                now.Clear();
            }
        }
        return true;
    }
    new private void Start()
    {
        base.Start();
        foreach (Channel cha in channels)
        {
            if (cha is OutChannel)
            {
                outChannel = cha as OutChannel;
            }
        }
    }
    private void Update()
    {
        if (outputs.Count > 0)
        {
            GameObject obj = outputs[0];
            var a = obj.transform.Find("Icon");
            Texture tmp = null;
            if (a != null)
            {
                var b = a.Find("RawImage");
                if (b != null)
                {
                    var c = b.gameObject.GetComponent<RawImage>();
                    if (c != null)
                    {
                        tmp = c.texture;
                    }
                }
            }
            outChannel.ChangeImage(tmp);
        }
    }

    private bool CheckSynthessisTable(out List<GameObject> ret)
    {
        ret = new();
        string excelPath = Path.Combine(Application.streamingAssetsPath, "SynthessisTable.xlsx");
        string excelSheetName = "Sleep";

        DataTable excelTableData = ReadExcel(excelPath, excelSheetName);

        int rows = excelTableData.Rows.Count;
        int columns = excelTableData.Columns.Count;

        var excelRowData = excelTableData.Rows;

        List<Kind> inputsKind = new();
        foreach (GameObject obj in inputs)
        {
            BaseObject tmp;
            if ((tmp = obj.GetComponent<BaseObject>()) != null)
            {
                inputsKind.Add(tmp.Kind);
            }
        }
        inputsKind.Sort();

        List<Kind> tmpret;
        for (int i = 1; i < rows; i++)
        {
            List<Kind> targetInputs = GetValues(excelRowData[i][0].ToString());

            if (inputsKind.SequenceEqual(targetInputs))
            {
                foreach (GameObject obj in inputs) Destroy(obj);
                ClearChannel();
                inputsKind.Clear();
                inputs.Clear();
                tmpret = GetValues(excelRowData[i][1].ToString());
                foreach (Kind x in tmpret) ret.Add(PrefabManager.Instance.GetObject(x));
                foreach (GameObject obj in ret) obj.SetActive(false);
                return true;
            }
        }
        return false;
    }
    private T FindInterface<T>(GameObject gameObject)
        where T : class
    {
        MonoBehaviour[] tmp = gameObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour obj in tmp)
        {
            if (obj is T)
            {
                return obj as T;
            }
        }
        return null;
    }
    DataTable ReadExcel(string excelPath, string excelSheet)
    {
        DataSet ret = null;
        using (FileStream fileStream = File.Open(excelPath, FileMode.Open, FileAccess.Read))
        {
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);

            ret = excelReader.AsDataSet();
        }
        return ret.Tables[excelSheet];
    }

    List<Kind> GetValues(string str)
    {
        List<Kind> ret = new();
        string[] now = str.Split(',');
        int result;
        foreach (string s in now)
        {
            if (int.TryParse(s, out result))
            {
                ret.Add((Kind)result);
            }
            ret.Sort();
        }
        return ret;
    }
}
