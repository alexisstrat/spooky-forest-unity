using System;
using System.Reflection;
using Spooky_Forest.Scripts.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution;

public class SettingsManager : MonoBehaviour
{
    [Header("Sound")]
    public TMP_Text soundOn;
    public TMP_Text soundOff;

    [Header("Shadows")] 
    public TMP_Text shadowsOn;
    public TMP_Text shadowsOff;
    
    [Header("Detail")]
    public TMP_Text detailHigh;
    public TMP_Text detailLow;
    

    [Header("Lock Rotation")]
    public TMP_Text lockRotationOn;
    public TMP_Text lockRotationOff;

    private FontStyles _upperCaseBold = FontStyles.Bold | FontStyles.UpperCase;
    private FontStyles _lowerCaseNormal = FontStyles.Normal | FontStyles.LowerCase;
    
    [Header("Font Colors")]
    public Color _onColor = Color.white;
    public Color _offColor= new Color(0.5849056f, 0.5849056f, 0.5849056f);
    
    private Type universalRenderPipelineAssetType;
    private FieldInfo mainLightShadowmapResolutionFieldInfo;

    private void InitializeShadowMapFieldInfo()
    {
        universalRenderPipelineAssetType = (GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset).GetType();
        mainLightShadowmapResolutionFieldInfo = universalRenderPipelineAssetType.GetField("m_MainLightShadowmapResolution", BindingFlags.Instance | BindingFlags.NonPublic);
    }

    /// <summary>
    /// Property accessor for shadow resolution using reflection
    /// </summary>
    public ShadowResolution MainLightShadowResolution
    {
        get
        {
            if (mainLightShadowmapResolutionFieldInfo == null)
            {
                InitializeShadowMapFieldInfo();
            }

            return (ShadowResolution) mainLightShadowmapResolutionFieldInfo.GetValue(GraphicsSettings
                .currentRenderPipeline);
        }
        set
        {
            if (mainLightShadowmapResolutionFieldInfo == null)
            {
                InitializeShadowMapFieldInfo();
            }

            mainLightShadowmapResolutionFieldInfo.SetValue(GraphicsSettings.currentRenderPipeline, value);
        }
    }
    

    /// <summary>
    /// Iterates through each one of the settings and
    /// sets the saved value
    /// </summary>
    public void SetSettings()
    {
        SetAudio();
        SetShadows();
        SetDetail();
        SetRotationLock();
    }

    /// <summary>
    /// Enables or pauses the audio listener in the scene
    /// </summary>
    private void SetAudio()
    {
        AudioManager.Get.AudioOnOff(GameManager.Get.gameData.Audio);
        if (GameManager.Get.gameData.Audio)
        {
            ChangeFontStyle(soundOff, soundOn);
        }
        else
        {
            ChangeFontStyle(soundOn, soundOff);
        }   
    }

    /// <summary>
    /// Enables or disables the shadows in the game
    /// </summary>
    private void SetShadows()
    {
        if (GameManager.Get.gameData.Shadows)
        {
            UniversalRenderPipeline.asset.shadowDistance = 15;
            ChangeFontStyle(shadowsOff, shadowsOn);
        }
        else
        {
            UniversalRenderPipeline.asset.shadowDistance = 0;
            ChangeFontStyle(shadowsOn, shadowsOff);
        }
    }

    /// <summary>
    /// Sets the level of detail of the game
    /// </summary>
    private void SetDetail()
    {
        if (GameManager.Get.gameData.HighDetail)
        {
            ChangeFontStyle(detailLow, detailHigh);
            MainLightShadowResolution = ShadowResolution._2048;
            UniversalRenderPipeline.asset.renderScale = .9f;
        }
        else
        {
            ChangeFontStyle(detailHigh, detailLow);
            MainLightShadowResolution = ShadowResolution._1024;
            UniversalRenderPipeline.asset.renderScale = 0.66f;
        }
    }

    /// <summary>
    /// Allows the game to rotate or not
    /// </summary>
    private void SetRotationLock()
    {
        DeviceOrientationManager.AutoRotateScreen = !GameManager.Get.gameData.LockRotation;
        Screen.autorotateToPortrait = !GameManager.Get.gameData.LockRotation;
        Screen.autorotateToLandscapeLeft = !GameManager.Get.gameData.LockRotation;
        Screen.autorotateToLandscapeRight = !GameManager.Get.gameData.LockRotation;

        if (GameManager.Get.gameData.LockRotation)
        {
            ChangeFontStyle(lockRotationOff, lockRotationOn);
        }
        else
        {
            ChangeFontStyle(lockRotationOn, lockRotationOff);
        }
    }
    
    public void ChangeAudio()
    {
        GameManager.Get.gameData.Audio = !GameManager.Get.gameData.Audio;
        SetAudio();
    }

    public void ChangeShadows()
    {
        GameManager.Get.gameData.Shadows = !GameManager.Get.gameData.Shadows;
        SetShadows();
    }
    
    public void ChangeDetail()
    {
        GameManager.Get.gameData.HighDetail = !GameManager.Get.gameData.HighDetail;
        SetDetail();
    }

    public void ChangeRotationLock()
    {
        GameManager.Get.gameData.LockRotation = !GameManager.Get.gameData.LockRotation;
        SetRotationLock();
    }
    
    /// <summary>
    /// Kinda flips the fontstyle of the given text elements
    /// </summary>
    /// <param name="text1">this one will always end up off</param>
    /// <param name="text2">this one will always end up on</param>
    void ChangeFontStyle(TMP_Text text1, TMP_Text text2)
    {
            text1.fontStyle = _lowerCaseNormal;
            text1.color = _offColor;
            
            text2.fontStyle = _upperCaseBold;
            text2.color = _onColor;
    }
}
