﻿using System;
using TMPro;
using UnityEngine;
using UI;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        
        public HUD hud;

        [SerializeField] private GameObject blurVolume;

        [Header("Game Win Screen")]
        [SerializeField] private GameObject gameWinUI;
        [SerializeField] private TextMeshProUGUI gameWinLandedText;
        
        [Header("Game Over Screen")]
        [SerializeField] private GameObject gameOverUI;
        [SerializeField] private TextMeshProUGUI gameOverLandedText;


        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            GameManager.Instance.onGameWin += ShowGameWin;
            GameManager.Instance.onGameOver += ShowGameOver;
        }

        private void ShowGameWin(int i)
        {
            gameWinLandedText.text = i.ToString("D3");
            ShowHUD(false);
            ShowBlur(true);
            gameWinUI.SetActive(true);
        }

        private void ShowGameOver(int i)
        {
            gameOverLandedText.text = i.ToString("D3");
            ShowHUD(false);
            ShowBlur(true);
            gameOverUI.SetActive(true);
        }

        public void ShowHUD(bool show)
        {
            hud.gameObject.SetActive(show);
        }

        public void ShowBlur(bool show)
        {
            blurVolume.SetActive(show);
        }
        
        // Button Click Functions

        public void BMainMenu()
        {
            
        }
        
        public void BRestart()
        {
            
        }
        
        public void BNextLevel()
        {
            
        }
    }
}