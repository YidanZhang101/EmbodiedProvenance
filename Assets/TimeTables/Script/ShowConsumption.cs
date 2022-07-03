using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowConsumption : MonoBehaviour
{
    public Texture2D colorRamp;

    Dictionary<string, int> monthly_92 = new Dictionary<string, int> 
    {
        {"January",50 },{"February", 60}, {"March",75 }, {"April",55 }, {"May",80 }, {"June",55 }, {"July",55 }, {"August",60 }, {"September",80 }, {"October",75 }, {"November",50 }, {"December",30 }
    };
    Dictionary<string, int> monthly_sport = new Dictionary<string, int>
    {
        {"January",45 },{"February", 50}, {"March",55 }, {"April",50 }, {"May",60 }, {"June",65 }, {"July",50 }, {"August",65 }, {"September",55 }, {"October",50 }, {"November",40 }, {"December",25 }
    };
    Dictionary<string, int> monthly_AT = new Dictionary<string, int>
    {
        {"January",30 },{"February", 40}, {"March",50 }, {"April",43 }, {"May",40 }, {"June",60 }, {"July",50 }, {"August",40 }, {"September",52 }, {"October",35 }, {"November",46 }, {"December",15 }
    };
    Dictionary<string, int> monthly_CC = new Dictionary<string, int>
    {
        {"January",50 },{"February", 60}, {"March",65 }, {"April",45 }, {"May",60 }, {"June",55 }, {"July",47 }, {"August",60 }, {"September",68 }, {"October",70 }, {"November",56 }, {"December",40 }
    };
    Dictionary<string, int> monthly_NH = new Dictionary<string, int>
    {
        {"January",60 },{"February", 68}, {"March",74 }, {"April",80 }, {"May",85 }, {"June",72 }, {"July",64 }, {"August",93 }, {"September",78 }, {"October",80 }, {"November",66 }, {"December",58 }
    };
    // all building feb weekly data
    Dictionary<string, int> weekly_92_feb = new Dictionary<string, int>{{"Week 1",40 },{"Week 2", 45}, {"Week 3",55 }, {"Week 4",60 }};
    Dictionary<string, int> weekly_sport_feb = new Dictionary<string, int> { { "Week 1", 45 }, { "Week 2", 50 }, { "Week 3", 45 }, { "Week 4", 40 } };
    Dictionary<string, int> weekly_AT_feb = new Dictionary<string, int> { { "Week 1", 50 }, { "Week 2", 45 }, { "Week 3", 42 }, { "Week 4", 35 } };
    Dictionary<string, int> weekly_CC_feb = new Dictionary<string, int> { { "Week 1", 45 }, { "Week 2", 50 }, { "Week 3", 84 }, { "Week 4", 76 } };
    Dictionary<string, int> weekly_NH_feb = new Dictionary<string, int> { { "Week 1", 67 }, { "Week 2", 65 }, { "Week 3", 70 }, { "Week 4", 72 } };
    // all building feb daily data
    Dictionary<string, int> daily_92_feb = new Dictionary<string, int> { { "Monday", 40 }, { "Tuesday", 45 }, { "Wednesday", 50 }, { "Thursday", 48 }, { "Friday", 43 }, { "Saturday", 28 }, { "Sunday", 25 } };
    Dictionary<string, int> daily_sport_feb = new Dictionary<string, int> { { "Monday", 35 }, { "Tuesday", 32 }, { "Wednesday", 28 }, { "Thursday", 40 }, { "Friday", 38 }, { "Saturday", 30 }, { "Sunday", 26 } };
    // all building feb hourly data
    Dictionary<string, int> hourly_92_feb = new Dictionary<string, int>
    {
        { "0am - 1am",8 },{ "1am - 2am",9 },{ "2am - 3am",10 },{ "3am - 4am",8 },{ "4am - 5am",10 },{ "5am - 6am",12 },{ "6am - 7am",12 },{ "7am - 8am",14 },{ "8am - 9am",20 },{ "9am - 10am",18 },{ "10am - 11am",25 },
        { "11am - 12pm",30 },{ "12pm - 13pm",28 },{ "13pm - 14pm",25 },{ "14pm - 15pm",22 },{ "15pm - 16pm",18 },{ "16pm - 17pm",15 },{ "17pm - 18pm",14 },{ "18pm - 19pm",16 },{ "19pm - 20pm",14 },{ "20pm - 21pm",11 },
        { "21pm - 22pm",10 }, {"22pm - 23pm",9 },{ "23pm - 24pm",8 }
    };
    Dictionary<string, int> hourly_sport_feb = new Dictionary<string, int>
    {
        { "0am - 1am",8 },{ "1am - 2am",9 },{ "2am - 3am",10 },{ "3am - 4am",8 },{ "4am - 5am",10 },{ "5am - 6am",12 },{ "6am - 7am",12 },{ "7am - 8am",14 },{ "8am - 9am",20 },{ "9am - 10am",18 },{ "10am - 11am",20 },
        { "11am - 12pm",24 },{ "12pm - 13pm",21 },{ "13pm - 14pm",23 },{ "14pm - 15pm",22 },{ "15pm - 16pm",23 },{ "16pm - 17pm",25 },{ "17pm - 18pm",27 },{ "18pm - 19pm",30 },{ "19pm - 20pm",34 },{ "20pm - 21pm",22 },
        { "21pm - 22pm",14 }, {"22pm - 23pm",8 },{ "23pm - 24pm",8 }
    };
    // all building june weekly data
    Dictionary<string, int> weekly_92_jun = new Dictionary<string, int> { { "Week 1", 45 }, { "Week 2", 50 }, { "Week 3", 40 }, { "Week 4", 42 } };
    Dictionary<string, int> weekly_sport_jun = new Dictionary<string, int> { { "Week 1", 50 }, { "Week 2", 50 }, { "Week 3", 50 }, { "Week 4", 45 } };
    Dictionary<string, int> weekly_AT_jun = new Dictionary<string, int> { { "Week 1", 50 }, { "Week 2", 45 }, { "Week 3", 55 }, { "Week 4", 56 } };
    Dictionary<string, int> weekly_CC_jun = new Dictionary<string, int> { { "Week 1", 45 }, { "Week 2", 50 }, { "Week 3", 40 }, { "Week 4", 43 } };
    Dictionary<string, int> weekly_NH_jun = new Dictionary<string, int> { { "Week 1", 67 }, { "Week 2", 68 }, { "Week 3", 70 }, { "Week 4", 72 } };
    // all building june daily data
    Dictionary<string, int> daily_92_jun = new Dictionary<string, int> { { "Monday", 30 }, { "Tuesday", 35 }, { "Wednesday", 40 }, { "Thursday", 38 }, { "Friday", 33 }, { "Saturday", 25 }, { "Sunday", 20 } };
    Dictionary<string, int> daily_sport_jun = new Dictionary<string, int> { { "Monday", 45 }, { "Tuesday", 38 }, { "Wednesday", 38 }, { "Thursday", 40 }, { "Friday", 45 }, { "Saturday", 30 }, { "Sunday", 36 } };
    // all building feb hourly data
    Dictionary<string, int> hourly_92_jun = new Dictionary<string, int>
    {
        { "0am - 1am",8 },{ "1am - 2am",9 },{ "2am - 3am",10 },{ "3am - 4am",8 },{ "4am - 5am",10 },{ "5am - 6am",12 },{ "6am - 7am",12 },{ "7am - 8am",14 },{ "8am - 9am",20 },{ "9am - 10am",18 },{ "10am - 11am",23 },
        { "11am - 12pm",26 },{ "12pm - 13pm",25 },{ "13pm - 14pm",25 },{ "14pm - 15pm",18 },{ "15pm - 16pm",21 },{ "16pm - 17pm",15 },{ "17pm - 18pm",14 },{ "18pm - 19pm",16 },{ "19pm - 20pm",14 },{ "20pm - 21pm",11 },
        { "21pm - 22pm",10 }, {"22pm - 23pm",9 },{ "23pm - 24pm",8 }
    };
    Dictionary<string, int> hourly_sport_jun = new Dictionary<string, int>
    {
        { "0am - 1am",8 },{ "1am - 2am",9 },{ "2am - 3am",10 },{ "3am - 4am",8 },{ "4am - 5am",10 },{ "5am - 6am",12 },{ "6am - 7am",12 },{ "7am - 8am",14 },{ "8am - 9am",20 },{ "9am - 10am",18 },{ "10am - 11am",20 },
        { "11am - 12pm",24 },{ "12pm - 13pm",21 },{ "13pm - 14pm",23 },{ "14pm - 15pm",22 },{ "15pm - 16pm",23 },{ "16pm - 17pm",25 },{ "17pm - 18pm",27 },{ "18pm - 19pm",30 },{ "19pm - 20pm",34 },{ "20pm - 21pm",22 },
        { "21pm - 22pm",14 }, {"22pm - 23pm",8 },{ "23pm - 24pm",8 }
    };
    // 2020 92 data
    Dictionary<string, int> monthly_92_2020 = new Dictionary<string, int>
    {
        {"January",50 },{"February", 60}, {"March",40 }, {"April",35 }, {"May",45 }, {"June",40 }, {"July",36 }, {"August",30 }, {"September",30 }, {"October",26 }, {"November",28 }, {"December",20 }
    };
    Dictionary<string, int> weekly_92_2020 = new Dictionary<string, int> { { "Week 1", 30 }, { "Week 2", 35 }, { "Week 3", 32 }, { "Week 4", 36 } }; // june weekly data
    Dictionary<string, int> daily_92_2020 = new Dictionary<string, int> { {"Monday",25 },{"Tuesday", 22}, {"Wednesday",21 }, {"Thursday",28 }, {"Friday",16 }, {"Saturday",14 }, {"Sunday",12 }};
    // Start is called before the first frame update
    void Start()
    {
        ChangeColor();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetConsumption(string buildingTag, Dictionary<string, int> buildingData)//(string month)
    {
        if (gameObject.tag == buildingTag)
        {
            foreach (KeyValuePair<string, int> kvp in buildingData)
            {
                if (gameObject.transform.GetChild(2).GetComponent<TextMeshPro>().text == kvp.Key)
                    gameObject.GetComponent<BuildingInteractor>().consumption = kvp.Value;
            }
        }
        
    }
    public void SetConsumption_Monthly_All()
    {
        SetConsumption("92", monthly_92);
        SetConsumption("Sport", monthly_sport);
        SetConsumption("AT", monthly_AT);
        SetConsumption("CC", monthly_CC);
        SetConsumption("NH", monthly_NH);
    }

    public void ChangeColor()
    {
        //GameObject a = GameObject.FindGameObjectWithTag("92");
        //a.GetComponent<BuildingInteractor>().consumption = 20;
        if (gameObject.transform.parent.gameObject.tag != "extractedLayer") // add monthly data
        {
            SetConsumption_Monthly_All();
            
            if (gameObject.transform.parent.parent.gameObject.name == "Layer2") // add 2020 92 data
            {
                SetConsumption("92", monthly_92_2020); // check monthly
                SetConsumption("92", weekly_92_2020); //check weekly
                SetConsumption("92", daily_92_2020);
            }
            else if (gameObject.transform.parent.parent.parent.gameObject.name == "LayerMonthly")
            {
                if (gameObject.transform.parent.parent.parent.gameObject.GetComponent<ParentTimeLevel>().parentTimeLevel == "February")
                {
                    SetConsumption("92", daily_92_feb);
                    SetConsumption("Sport", daily_sport_feb);
                }
                if (gameObject.transform.parent.parent.parent.gameObject.GetComponent<ParentTimeLevel>().parentTimeLevel == "June")
                {
                    SetConsumption("92", daily_92_jun);
                    SetConsumption("Sport", daily_sport_jun);
                }

            }
            else if (gameObject.transform.parent.gameObject.name == "Hourly")//parent.parent.parent.gameObject.name == "LayerMonthly")
            {
                if (gameObject.transform.parent.parent.parent.parent.gameObject.GetComponent<ParentTimeLevel>().parentTimeLevel == "February")
                {
                    SetConsumption("92", hourly_92_feb);
                    SetConsumption("Sport", hourly_sport_feb);
                }
                if (gameObject.transform.parent.parent.parent.parent.gameObject.GetComponent<ParentTimeLevel>().parentTimeLevel == "June")
                {
                    SetConsumption("92", hourly_92_jun);
                    SetConsumption("Sport", hourly_sport_jun);
                }

            }
            else
            {
                if (gameObject.transform.parent.parent.gameObject.name != "Tabletop")
                {
                    if (gameObject.transform.parent.parent.gameObject.GetComponent<ParentTimeLevel>().parentTimeLevel == "February") // add all layer feb weekly data
                    {
                        SetConsumption("92", weekly_92_feb);
                        SetConsumption("Sport", weekly_sport_feb);
                        SetConsumption("AT", weekly_AT_feb);
                        SetConsumption("CC", weekly_CC_feb);
                        SetConsumption("NH", weekly_NH_feb);
                    }
                    if (gameObject.transform.parent.parent.gameObject.GetComponent<ParentTimeLevel>().parentTimeLevel == "June") // add all layer june weekly data
                    {
                        SetConsumption("92", weekly_92_jun);
                        SetConsumption("Sport", weekly_sport_jun);
                        SetConsumption("AT", weekly_AT_jun);
                        SetConsumption("CC", weekly_CC_jun);
                        SetConsumption("NH", weekly_NH_jun);
                    }
                }
            }
        }
                  
        int consumption = gameObject.GetComponent<BuildingInteractor>().consumption;
        print(consumption);
        gameObject.GetComponent<Renderer>().material.color = GetColor(consumption);
    }

    public Color GetColor(int value)
    {
        //normalise the value
        float normalisedValue = value / 100f;

        //interpolate pixel width
        int w = (int)Mathf.Lerp(0, 1000f, normalisedValue);

        //get pixel color
        Color color = colorRamp.GetPixel(w, 0);
        return color;
    }
}
