using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Anvil : BaseObject, IReceive, IGive
{
    List<GameObject> inMaterials = new();
    List<GameObject> inHardworks = new();
    List<GameObject> outputs = new();
    [SerializeField] List<Channel> channels = new();
    Channel outChannel;
    [SerializeField] CircleTimer timer;
    [SerializeField] int maxTime;
    [SerializeField] float maxInfluence;
    [SerializeField] Animator animator;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] audioClips;
    #region IReceive
    public bool ReceiveObject(GameObject rec)
    {
        BaseObject baseObject = rec.GetComponent<BaseObject>();

        if (baseObject.HasTag(Feature.IsHardwork)) inHardworks.Add(rec);
        else if (baseObject.HasTag(Feature.IsMaterial)) inMaterials.Add(rec);
        else return false;

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
            if (inHardworks.Count > 0)
            {
                GameManager.Instance.nowGameObject = inHardworks[0];
                GameManager.Instance.nowGameObject.SetActive(true);
                inHardworks.RemoveAt(0);
                return true;
            }
            if (inMaterials.Count > 0)
            {
                GameManager.Instance.nowGameObject = inMaterials[0];
                GameManager.Instance.nowGameObject.SetActive(true);
                inMaterials.RemoveAt(0);
                return true;
            }
        }
        else
        {
            Kind tmp;
            GameObject target = null;
            for (int i = 0; i < inHardworks.Count; i++)
            {
                tmp = BaseOpe.GetKind(inHardworks[i]);
                if (tmp == which && target == null)
                {
                    target = inHardworks[i];
                    inHardworks.RemoveAt(i);
                }
            }
            for (int i = 0; i < inMaterials.Count; i++)
            {
                tmp = BaseOpe.GetKind(inMaterials[i]);
                if (tmp == which && target == null)
                {
                    target = inMaterials[i];
                    inMaterials.RemoveAt(i);
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
        if (outputs.Count == 0)
        {
            timer.InterruptTimer();
        } 
        else
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
        if (timer.Working)
        {
            foreach (GameObject obj in outputs)
            {
                BaseObject tmp = obj.GetComponent<BaseObject>();
                if (tmp != null) tmp.crispness += maxInfluence / maxTime * Time.deltaTime;
            }
        }
        animator.SetBool("isWorking", timer.Working);
    }
    private bool CheckSynthessisTable(out List<GameObject> ret)
    {
        ret = new();
        string excelPath = Path.Combine(Application.streamingAssetsPath, "SynthessisTable.xlsx");
        string excelSheetName = "Anvil";

        DataTable excelTableData = ReadExcel(excelPath, excelSheetName);

        int rows = excelTableData.Rows.Count;
        int columns = excelTableData.Columns.Count;

        var excelRowData = excelTableData.Rows;

        List<Kind> materials = new();
        foreach (GameObject obj in inMaterials)
        {
            BaseObject tmp;
            if ((tmp = obj.GetComponent<BaseObject>()) != null)
            {
                materials.Add(tmp.Kind);
            }
        }
        materials.Sort();

        List<Kind> hardworks = new();
        foreach (GameObject obj in inHardworks)
        {
            BaseObject tmp;
            if ((tmp = obj.GetComponent<BaseObject>()) != null)
            {
                hardworks.Add(tmp.Kind);
            }
        }
        hardworks.Sort();

        List<Kind> tmpret;
        for (int i = 1; i < rows; i++)
        {
            List<Kind> targetMaterials = GetValues(excelRowData[i][0].ToString());
            List<Kind> targetHardworks = GetValues(excelRowData[i][1].ToString());

            if (materials.SequenceEqual(targetMaterials) && hardworks.SequenceEqual(targetHardworks))
            {
                foreach (GameObject obj in inMaterials) Destroy(obj);
                foreach (GameObject obj in inHardworks) Destroy(obj);
                ClearChannel();
                materials.Clear();
                hardworks.Clear();
                inMaterials.Clear();
                inHardworks.Clear();
                tmpret = GetValues(excelRowData[i][2].ToString());

                audioSource.clip = audioClips[UnityEngine.Random.Range(0, audioClips.Length - 1)];
                audioSource.Play();

                timer.StartTimer(maxTime);
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
        foreach (string s in now)
        {
            if (s != "") ret.Add((Kind)Convert.ToInt32(s.Trim()));
        }
        ret.Sort();
        return ret;
    }
}
