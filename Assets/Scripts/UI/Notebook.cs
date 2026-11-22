
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Notebook : MonoBehaviour
{
    public static Notebook Instance;
    public Image BookBack;
    public List<Image> Pages = new List<Image>();
    public int _currentBookIndex;

    public float FadeDuration = 1.4f;
    public bool IsBookOpen = false;

    public Button[] Buttons;

    private Vector3 _baseScale;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {

        _baseScale = Pages[0].gameObject.transform.localScale;
        _currentBookIndex = Pages.Count - 1;

        foreach(var item in Pages)
        {
            if(item.GetComponent<Page>().HasBeenFound == false)
            {
                item.DOFade(0, 0);
                item.GetComponentInChildren<TextMeshProUGUI>().DOFade(0, 0);
                item.gameObject.transform.DOScale(_baseScale * 1.2f, 0);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (IsBookOpen == false)
            {
                IsBookOpen = true;
                ShowCurrentPages();
            }
            else
            {
                IsBookOpen = false;
                HideBook();
            }
           
        }
    }

    public void ShowCurrentPages()
    {
        IsBookOpen = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        foreach (Button b in Buttons) 
        { 
            b.gameObject.SetActive(true);
        }
        if(_currentBookIndex == 0)
        {
            Buttons[0].gameObject.SetActive(false);
        }
        if (_currentBookIndex >= Pages.Count - 1) 
        {
            Buttons[1].gameObject.SetActive(false);
        }

        for(int i = _currentBookIndex; i < 0; i--)
        {
            Pages[i].gameObject.SetActive(false);
        }



        BookBack.gameObject.SetActive(true);

        if (Pages[_currentBookIndex].GetComponent<Page>().HasBeenFound)
        {
            Pages[_currentBookIndex].gameObject.SetActive(true);
        }
        else
        {
            if(_currentBookIndex+2 < Pages.Count )
            {
                if (Pages[_currentBookIndex+2].GetComponent<Page>().HasBeenFound)
                {
                    Pages[_currentBookIndex+2].gameObject.SetActive(true);
                }
                if (_currentBookIndex + 4 < Pages.Count )
                {
                    if (Pages[_currentBookIndex + 4].GetComponent<Page>().HasBeenFound)
                    {
                        Pages[_currentBookIndex + 4].gameObject.SetActive(true);
                    }
                    if (_currentBookIndex + 6 < Pages.Count )
                    {
                        if (Pages[_currentBookIndex + 6].GetComponent<Page>().HasBeenFound)
                        {
                            Pages[_currentBookIndex + 6].gameObject.SetActive(true);
                        }
                        if (_currentBookIndex + 8 < Pages.Count )
                        {
                            if (Pages[_currentBookIndex + 8].GetComponent<Page>().HasBeenFound)
                            {
                                Pages[_currentBookIndex + 8].gameObject.SetActive(true);
                            }
                        }
                    }
                }
            }
        }


        if (_currentBookIndex < Pages.Count - 1)
        {
           
            if (Pages[_currentBookIndex+1].GetComponent<Page>().HasBeenFound)
            {
                Pages[_currentBookIndex + 1].gameObject.SetActive(true);
            }
            else
            {
                if (_currentBookIndex + 3 < Pages.Count - 1)
                {
                    if (Pages[_currentBookIndex + 3].GetComponent<Page>().HasBeenFound)
                    {
                        Pages[_currentBookIndex + 3].gameObject.SetActive(true);
                    }
                    if (_currentBookIndex + 5 < Pages.Count - 1)
                    {
                        if (Pages[_currentBookIndex + 5].GetComponent<Page>().HasBeenFound)
                        {
                            Pages[_currentBookIndex + 5].gameObject.SetActive(true);
                        }
                        if (_currentBookIndex + 7 < Pages.Count - 1)
                        {
                            if (Pages[_currentBookIndex + 7].GetComponent<Page>().HasBeenFound)
                            {
                                Pages[_currentBookIndex + 7].gameObject.SetActive(true);
                            }
                            if (_currentBookIndex + 9 < Pages.Count - 1)
                            {
                                if (Pages[_currentBookIndex + 9].GetComponent<Page>().HasBeenFound)
                                {
                                    Pages[_currentBookIndex + 9].gameObject.SetActive(true);
                                }
                            }
                        }
                    }
                }
            }
        }
        
    }


    public void CollectPage(int pageIndex)
    {
        Pages[pageIndex].GetComponent<Page>().HasBeenFound = true;
        Pages[pageIndex].DOFade(1,FadeDuration);
        Pages[pageIndex].transform.DOScale(_baseScale, FadeDuration);
        Pages[pageIndex].GetComponentInChildren<TextMeshProUGUI>().DOFade(1, FadeDuration);
        if(pageIndex%2 != 0 && pageIndex !=0)
        {
            _currentBookIndex = pageIndex-1;
        }
        else
        {
            _currentBookIndex = pageIndex;
        }
          
        ShowCurrentPages();
    }
    public void OnClickLeftPage()
    {
        Buttons[1].gameObject.SetActive(true);
        Pages[_currentBookIndex].gameObject.SetActive(false);
        if (_currentBookIndex<Pages.Count-1)
        {
            Pages[_currentBookIndex + 1].gameObject.SetActive(false);
        }
       

        _currentBookIndex -= 2;
        if (_currentBookIndex <0)
        {
            _currentBookIndex =0;
            Buttons[0].gameObject.SetActive(false);
        }

            ShowCurrentPages();
    }

    public void OnClickRightPage()
    {
        Buttons[0].gameObject.SetActive(true);
        Pages[_currentBookIndex].gameObject.SetActive(false);
        Pages[_currentBookIndex + 1].gameObject.SetActive(false);

        _currentBookIndex += 2;
        if (_currentBookIndex >= Pages.Count)
        {
            _currentBookIndex = Pages.Count - 1;
            Buttons[1].gameObject.SetActive(false);
        }
        ShowCurrentPages();
    }


    public void HideBook()
    {
        foreach (var page in Pages)
        {
            page.gameObject.SetActive(false);
        }

        foreach (Button b in Buttons)
        {
            b.gameObject.SetActive(false);
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        IsBookOpen = false;
        BookBack.gameObject.SetActive(false);
    }
}
