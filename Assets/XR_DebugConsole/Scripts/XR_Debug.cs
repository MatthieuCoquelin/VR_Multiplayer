using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XR_Debug : MonoBehaviour
{
    /// <summary>
    /// Max log number
    /// </summary>
    [SerializeField] private int m_maxStack = 50;
    
    /// <summary>
    /// Max length log
    /// </summary>
    [SerializeField] private int m_maxLength = 100;

    /// <summary>
    /// Button prefab (one by log type)
    /// </summary>
    [SerializeField] private Button_Log m_logButton = null;
    [SerializeField] private Button_LogError m_logErrorButton = null;
    [SerializeField] private Button_LogWarning m_logWarningButton = null;

    /// <summary>
    /// list of log
    /// </summary>
    [SerializeField] private Transform m_origin = null;

    /// <summary>
    /// on right panel put the full log
    /// </summary>
    [SerializeField] private TextMeshProUGUI m_fullText = null;

    /// <summary>
    /// on top panel we choose or not to display the logs by hidding panels 
    /// </summary>
    [SerializeField] private TextMeshProUGUI m_toggleText = null;
    [SerializeField] private Toggle m_toggle = null;
    [SerializeField] private GameObject m_leftPanel = null;
    [SerializeField] private GameObject m_rightPanel = null;

    /// <summary>
    /// Text of buttons's logs
    /// </summary>
    [SerializeField] private TextMeshProUGUI m_logFilterText = null;
    [SerializeField] private TextMeshProUGUI m_errorFilterText = null;
    [SerializeField] private TextMeshProUGUI m_warningFilterText = null;

    /// <summary>
    /// list of buttons
    /// </summary>
    private List<Base_Log> m_logsList = new List<Base_Log>();
    private List<Button_Log> m_logList = new List<Button_Log>();
    private List<Button_LogError> m_logErrorList = new List<Button_LogError>();
    private List<Button_LogWarning> m_logWarningList = new List<Button_LogWarning>();

    /// <summary>
    /// bool value to filter the logs
    /// </summary>
    private bool m_logFilterState = false;
    private bool m_errorFilterState = false;
    private bool m_warningFilterState = false;

    /// <summary>
    /// reference to button selcted regardless its type 
    /// </summary>
    private Base_Log m_logButtonClicked = null;

    private void Awake()
    {
        // if no logs
        m_logFilterText.text = "0";
        m_errorFilterText.text = "0";
        m_warningFilterText.text = "0";
    }

    void OnEnable()
    {
        //called at each log
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void Start()
    {
        OnToogleValueChange();
        m_logFilterState = false;
        m_errorFilterState = false;
        m_warningFilterState = false;
        m_fullText.text = "";
    }

    /// <summary>
    /// click / unclick button log
    /// </summary>
    /// <param name="value"></param>
    public void UpdateColorLogButton(bool value)
    {
        Button btn = m_logButtonClicked.gameObject.GetComponent<Button>();
        btn.image.color = value ? btn.colors.pressedColor : btn.colors.normalColor + Color.white;
    }

    /// <summary>
    /// update color button and display full text on right panel
    /// </summary>
    /// <param name="baseLog"></param>
    /// <param name="txt"></param>
    private void OnButtonClicked(Base_Log baseLog, string txt)
    {
        if(m_logButtonClicked) 
            UpdateColorLogButton(false);
        m_logButtonClicked = baseLog;
            UpdateColorLogButton(true);
        m_fullText.text = txt;
    }

    private void ComptuteLogsInstance(Base_Log baseLog, string log)
    {
        // make a button clickable 
        baseLog.gameObject.GetComponent<Button>().onClick.AddListener(() => OnButtonClicked(baseLog, log));

        // update full log
        baseLog.FullLog = log;

        // apply max length on a button text
        if (log.Length > m_maxLength)
            baseLog.ShortedLog = log.Substring(0, m_maxLength);
        else
            baseLog.ShortedLog = log.Substring(0, log.Length);

        // put the log on the button and save it
        baseLog.Text.text = baseLog.ShortedLog;
        m_logsList.Add(baseLog);
    }


    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // limit the logs 
        if (m_logsList.Count > m_maxStack)
            return;

        string newString = "[" + type + "] : " + logString;

        // for eack log we intantiate a button initialize it and class button in lists
        if (type == LogType.Log)
        {
            var logInstance = Instantiate(m_logButton, m_origin);
            ComptuteLogsInstance(logInstance, newString);
            m_logList.Add(logInstance);
            m_logFilterText.text = m_logList.Count.ToString();
        }
        else if (type == LogType.Error)
        {
            var errorInstance = Instantiate(m_logErrorButton, m_origin);
            ComptuteLogsInstance(errorInstance, newString);
            m_logErrorList.Add(errorInstance);
            m_errorFilterText.text = m_logErrorList.Count.ToString();
        }
        else if (type == LogType.Warning)
        {
            var warningInstance = Instantiate(m_logWarningButton, m_origin);
            ComptuteLogsInstance(warningInstance, newString);
            m_logWarningList.Add(warningInstance);
            m_warningFilterText.text = m_logWarningList.Count.ToString();
        }
    }

    /// <summary>
    /// button filter for each log type
    /// </summary>
    /// <param name="btn"></param>
    /// <param name="value"></param>
    protected void UpdadeColorFilterButton(Button btn, bool value)
    {
        if (btn.gameObject.name == "LogFilterButton")
            m_logFilterState = value;
        else if (btn.gameObject.name == "LogErrorFilterButton")
            m_errorFilterState = value;
        else if (btn.gameObject.name == "LogWarningFilterButton")
            m_warningFilterState = value;

        //need to add standard image color for normal color to have original color 
        btn.image.color = value ? btn.colors.pressedColor : btn.colors.normalColor + Color.white;
    }

    /// <summary>
    /// Reset text display on click on filter
    /// </summary>
    private void ResetLogButtonClicked()
    {
        if (m_logButtonClicked)
            UpdateColorLogButton(false);
        m_logButtonClicked = null;

        m_fullText.text = "";
    }

    /// <summary>
    /// Log filter clicked
    /// </summary>
    public void OnLogFilterClicked()
    {
        bool value = true;
        if (m_logFilterState)
            value = false;
        for (int i = 0; i < m_logList.Count; i++)
        {
            m_logList[i].gameObject.SetActive(!m_logList[i].gameObject.activeSelf);
        }
        UpdadeColorFilterButton(m_logFilterText.transform.parent.GetComponent<Button>(), value);
        ResetLogButtonClicked();
    }

    /// <summary>
    /// Log Error filter Clicked
    /// </summary>
    public void OnLogErrorFilterClicked()
    {
        bool value = true;
        if (m_errorFilterState)
            value = false;
        for (int i = 0; i < m_logErrorList.Count; i++)
        {
            m_logErrorList[i].gameObject.SetActive(!m_logErrorList[i].gameObject.activeSelf);
        }
        UpdadeColorFilterButton(m_errorFilterText.transform.parent.GetComponent<Button>(), value);
        ResetLogButtonClicked();
    }

    /// <summary>
    /// Log Warning filter clicked
    /// </summary>
    public void OnLogWarningFilterClicked()
    {
        bool value = true;
        if (m_warningFilterState)
            value = false;
        for (int i = 0; i < m_logWarningList.Count; i++)
        {
            m_logWarningList[i].gameObject.SetActive(!m_logWarningList[i].gameObject.activeSelf);
        }
        UpdadeColorFilterButton(m_warningFilterText.transform.parent.GetComponent<Button>(), value);
        ResetLogButtonClicked();
    }

    /// <summary>
    /// Display the log panel or not 
    /// </summary>
    public void OnToogleValueChange()
    {
        if(m_toggle.isOn)
        {
            m_toggleText.text = "Hide";
            m_leftPanel.SetActive(true);
            m_rightPanel.SetActive(true);
        }
        else
        {
            m_toggleText.text = "Show";
            m_leftPanel.SetActive(false);
            m_rightPanel.SetActive(false);
        }
    }


}
