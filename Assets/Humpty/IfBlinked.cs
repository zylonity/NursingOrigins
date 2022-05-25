namespace OpenCvSharp.Demo
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.Rendering.PostProcessing;


    public class IfBlinked : MonoBehaviour
    {

        [HideInInspector]
        public bool lookingAtColl = false;

        [HideInInspector]
        public string lookingAt = null;

        public Animator blink;
        public GameObject webcamHandler;

        public GameObject player;
        public PlayerController pControl;
        float mouseSens;

        bool sceneReady = false;

        public AudioSource audioSource;
        public PostProcessVolume camVol;

        Vignette vig;
        ChromaticAberration cA;
        ColorGrading cG;

        [Header(" --- Stage 1")]
        public GameObject Stage1;
        public AudioClip audOne;
        public float Stage1Time;
        public float maxVigAndCaIncS1;
        public GameObject blinkCanvasS11;
        public GameObject S1Army;
        [ColorUsageAttribute(false, true)]
        public Color colorShiftS1;

        [Space(10)]
        [Header(" --- Stage 2")]
        public GameObject Stage2;
        public AudioClip audTwo;
        public float Stage2Time;
        public float maxVigAndCaIncS2;
        public CannonBall ballS21, ballS22, ballS23;
        public GameObject blinkCanvasS21;
        [ColorUsageAttribute(false, true)]
        public Color colorShiftS2;        

        [Space(10)]
        [Header(" --- Stage 3")]
        public GameObject Stage3;
        public AudioClip audThree;
        public float Stage3Time;
        public float maxVigAndCaIncS3;
        public GameObject blinkCanvasS31;
        public GameObject blinkCanvasS32;
        [ColorUsageAttribute(false, true)]
        public Color colorShiftS3;

        [Space(10)]
        [Header(" --- Stage 4")]
        public GameObject Stage4;
        public GameObject S4Player;
        public AudioSource S4audioSource;
        public AudioClip audFour;
        public float Stage4Time;
        public float maxVigAndCaIncS4;
        [ColorUsageAttribute(false, true)]
        public Color colorShiftS4;

        [Space(10)]
        [Header(" --- Stage 5")]
        public GameObject Stage5;
        public GameObject S5Player;
        public AudioSource S5audioSource;
        public float Stage5Time;
        public float maxVigAndCaIncS5;
        [ColorUsageAttribute(false, true)]
        public Color colorShiftS5;


        int stage = 0;
        float counter = 0;
        float tCounter = 1;
        float maxSlow = 0.2f;

        float tempMouse;
        float maxMouseMultiplier = 0.05f;

        public bool gameStart = false;
        bool firstRun = false;

        float ogInt;
        Color ogCol;

        FaceDetectorScene faceDect;

        void OnEnable()
        {
            BlinkDect.onBlink += bBlinked;



            camVol.profile.TryGetSettings(out vig);
            camVol.profile.TryGetSettings(out cA);
            camVol.profile.TryGetSettings(out cG);
            ogInt = cA.intensity.value;
            ogCol = cG.colorFilter.value;

            faceDect = webcamHandler.GetComponentInChildren<FaceDetectorScene>();


        }

        private void Start()
        {
            mouseSens = pControl.mouseSens;
            tempMouse = mouseSens;
            stage = 1;
        }

        private void Update()
        {

            if (stage == 1 && gameStart)
            {
                if (!firstRun)
                {
                    audioSource.clip = audOne;
                    audioSource.Play();
                    firstRun = true;
                }

                counter += Time.unscaledDeltaTime;
                Time.timeScale = tCounter;
                pControl.mouseSens = tempMouse;
                if (counter < Stage1Time)
                {

                    if (tCounter > maxSlow)
                    {
                        tCounter -= (counter / Stage1Time) / 100f;
                    }
                    else
                        tCounter = maxSlow;

                    if (pControl.mouseSens > mouseSens * maxMouseMultiplier)
                    {
                        tempMouse -= (mouseSens / Stage1Time) / 100f;
                    }



                    vig.intensity.Interp(ogInt, ogInt + maxVigAndCaIncS1, counter / Stage1Time);
                    cA.intensity.Interp(ogInt, ogInt + maxVigAndCaIncS1, counter / Stage1Time);
                    cG.colorFilter.Interp(ogCol, colorShiftS1, counter / Stage1Time);


                }
                else
                {
                    tCounter = maxSlow;
                    blinkCanvasS11.SetActive(true);
                    faceDect.processingC = true;
                    sceneReady = true;
                }
            }
            else if (stage == 2)
            {
                counter += Time.unscaledDeltaTime;
                Time.timeScale = tCounter;
                pControl.mouseSens = tempMouse;
                if (counter < Stage2Time)
                {
                    ballS21.FireAndBreak();
                    ballS22.FireAndBreak();
                    ballS23.FireAndBreak();

                    if (tCounter > maxSlow)
                    {
                        tCounter -= (counter / Stage2Time) / 100f;
                    }
                    else
                        tCounter = maxSlow;

                    if (pControl.mouseSens > mouseSens * maxMouseMultiplier)
                    {
                        tempMouse -= (mouseSens / Stage1Time) / 100f;
                    }

                    vig.intensity.Interp(ogInt, ogInt + maxVigAndCaIncS2, counter / Stage2Time);
                    cA.intensity.Interp(ogInt, ogInt + maxVigAndCaIncS2, counter / Stage2Time);
                    cG.colorFilter.Interp(ogCol, colorShiftS2, counter / Stage2Time);

                }
                else
                {
                    blinkCanvasS21.SetActive(true);
                    faceDect.processingC = true;
                    sceneReady = true;
                }
            }
            else if (stage == 3)
            {
                counter += Time.unscaledDeltaTime;
                Time.timeScale = tCounter;
                pControl.mouseSens = tempMouse;
                if (counter < Stage3Time)
                {

                    if (tCounter > maxSlow)
                    {
                        tCounter -= (counter / Stage3Time) / 100f;
                    }
                    else
                        tCounter = maxSlow;

                    if (pControl.mouseSens > mouseSens * maxMouseMultiplier)
                    {
                        tempMouse -= (mouseSens / Stage1Time) / 100f;
                    }

                    vig.intensity.Interp(ogInt, ogInt + maxVigAndCaIncS3, counter / Stage3Time);
                    cA.intensity.Interp(ogInt, ogInt + maxVigAndCaIncS3, counter / Stage3Time);
                    cG.colorFilter.Interp(ogCol, colorShiftS3, counter / Stage3Time);


                }
                else
                {
                    tCounter = maxSlow;
                    blinkCanvasS31.SetActive(true);
                    blinkCanvasS32.SetActive(true);
                    faceDect.processingC = true;
                    sceneReady = true;
                }





            }
            else if (stage == 4)
            {
                counter += Time.unscaledDeltaTime;
                Time.timeScale = tCounter;
                pControl.mouseSens = tempMouse;
                if (counter < Stage4Time)
                {

                    if (tCounter > maxSlow)
                    {
                        tCounter -= (counter / Stage4Time) / 100f;
                    }
                    else
                        tCounter = maxSlow;

                    if (pControl.mouseSens > mouseSens * maxMouseMultiplier)
                    {
                        tempMouse -= (mouseSens / Stage4Time) / 100f;
                    }

                    vig.intensity.Interp(ogInt, ogInt + maxVigAndCaIncS4, counter / Stage4Time);
                    cA.intensity.Interp(ogInt, ogInt + maxVigAndCaIncS4, counter / Stage4Time);
                    cG.colorFilter.Interp(ogCol, colorShiftS4, counter / Stage4Time);


                }
                else
                {
                    tCounter = maxSlow;
                    sceneReady = true;
                    blink.SetTrigger("Blink");
                    StartCoroutine(HumptyEnd());
                }
            }
            else if (stage == 5)
            {
                counter += Time.unscaledDeltaTime;
                Time.timeScale = tCounter;
                pControl.mouseSens = tempMouse;
                if (counter < Stage5Time)
                {

                    if (tCounter > maxSlow)
                        tCounter -= (counter / Stage5Time) / 100f;
                    else
                        tCounter = maxSlow;

                    if (pControl.mouseSens > mouseSens * maxMouseMultiplier)
                    {
                        tempMouse -= (mouseSens / Stage5Time) / 100f;
                    }


                    vig.intensity.Interp(ogInt, maxVigAndCaIncS5, counter / Stage5Time);
                    cA.intensity.Interp(ogInt, maxVigAndCaIncS5, counter / Stage5Time);
                    cG.colorFilter.Interp(ogCol, colorShiftS5, counter / Stage5Time);


                }
                else
                {
                    tCounter = maxSlow;
                    sceneReady = true;
                    blink.SetTrigger("Blink");
                    StartCoroutine(HumptyEnd());
                }
            }
        }


        void OnDisable()
        {
            BlinkDect.onBlink -= bBlinked;
        }

        void bBlinked()
        {

            if (sceneReady && lookingAtColl)
            {
                if (stage == 1)
                    Stage1End();
                else if (stage == 2)
                    Stage2End();
                else if (stage == 3)
                {
                    if (lookingAt == "S3PointerDEAD")
                    {
                        Stage3DEAD();
                        print("dead");
                    }
                    else if (lookingAt == "S3PointerALIVE")
                    {
                        Stage3ALIVE();
                        print("Alive");
                    }

                }


            }



        }

        void Stage1End()
        {
            Stage1.SetActive(false);
            faceDect.processingC = false;
            lookingAtColl = false;
            Stage2.SetActive(true);
            audioSource.clip = audTwo;
            audioSource.Play();
            sceneReady = false;
            stage = 2;
            counter = 0;
            tempMouse = mouseSens;
            ogInt = cA.intensity;
            ogCol = cG.colorFilter;
        }

        void Stage2End()
        {
            Stage2.SetActive(false);
            faceDect.processingC = false;
            lookingAtColl = false;
            Stage3.SetActive(true);
            audioSource.clip = audThree;
            audioSource.Play();
            sceneReady = false;
            stage = 3;
            counter = 0;
            tempMouse = mouseSens;
            tCounter = 0.8f;
            maxSlow = 0.2f;
            ogInt = cA.intensity;
            ogCol = cG.colorFilter;
        }

        void Stage3DEAD()
        {
            Stage3.SetActive(false);
            faceDect.processingC = false;
            lookingAtColl = false;
            Stage4.SetActive(true);
            player.SetActive(false);
            pControl = S4Player.GetComponentInChildren<PlayerController>();
            S4audioSource.clip = audFour;
            S4audioSource.Play();
            sceneReady = false;
            stage = 4;
            counter = 0;
            tempMouse = mouseSens;
            tCounter = 1;
            maxSlow = 0.2f;
            ogInt = cA.intensity;
            ogCol = cG.colorFilter;
        }

        void Stage3ALIVE()
        {
            Stage3.SetActive(false);
            faceDect.processingC = false;
            lookingAtColl = false;
            Stage5.SetActive(true);
            player.SetActive(false);
            pControl = S5Player.GetComponentInChildren<PlayerController>();
            S5audioSource.clip = audFour;
            S5audioSource.Play();
            sceneReady = false;
            stage = 5;
            counter = 0;
            tempMouse = mouseSens;
            tCounter = 1;
            maxSlow = 0.2f;
            ogInt = cA.intensity;
            ogCol = cG.colorFilter;
        }

        IEnumerator HumptyEnd()
        {
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(4);
            yield break;

        }
    }
}

