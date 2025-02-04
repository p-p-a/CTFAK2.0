﻿using CTFAK.CCN.Chunks.Objects;
using CTFAK.FileReaders;
using CTFAK.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumper
{
    class SortedImageDumper : IFusionTool
    {
        public string Name => "Sorted Image Dumper";
        int imageNumber = 0;

        public void Execute(IFileReader reader)
        {
            var outPath = reader.getGameData().name ?? "Unknown Game";
            var images = reader.getGameData().Images.Items;
            var frames = reader.getGameData().frames;
            var objects = reader.getGameData().frameitems;
            
            foreach (var frame in frames)
            {
                string frameFolder = $"Dumps\\{outPath}\\Sorted Images\\{Utils.ClearName(frame.name)}\\";
                foreach (var instance in frame.objects)
                {
                    
                    var oi = objects[instance.objectInfo];
                    var objectFolder = frameFolder+Utils.ClearName(oi.name)+"\\";
                    if (oi.properties is Backdrop bg)
                    {
                        Directory.CreateDirectory(objectFolder);
                        Directory.CreateDirectory($"{frameFolder}{{unsorted}}");
                        images[bg.Image]?.bitmap.Save(objectFolder + "0.png");
                        images[bg.Image]?.bitmap.Save($"{frameFolder}{{unsorted}}\\{imageNumber}.png");
                        imageNumber++;
                    }
                    else if (oi.properties is Quickbackdrop qbg)
                    {
                        Directory.CreateDirectory(objectFolder);
                        Directory.CreateDirectory($"{frameFolder}{{unsorted}}");
                        images[qbg.Image]?.bitmap.Save(objectFolder + "0.png");
                        images[qbg.Image]?.bitmap.Save($"{frameFolder}{{unsorted}}\\{imageNumber}.png");
                        imageNumber++;
                    }
                    else if(oi.properties is ObjectCommon common)
                    {
                        if(common.Parent.ObjectType==2)
                        {
                            int cntrAnims = 0;
                            foreach (var anim in common.Animations?.AnimationDict)
                            {
                                if (anim.Value.DirectionDict?.Count > 0) cntrAnims++;
                            }
                            foreach (var anim in common.Animations?.AnimationDict)
                            {
                                string animationFolder = "";
                                if (cntrAnims>0) animationFolder = objectFolder + $"Animation {anim.Key}\\"; 
                                else animationFolder = objectFolder;

                                int cntrDirs = 0;




                                if (anim.Value.DirectionDict == null) continue;
                                foreach (var dir in anim.Value?.DirectionDict)
                                {
                                    if (dir.Value.Frames.Count > 0) cntrDirs++;
                                }
                                foreach (var dir in anim.Value?.DirectionDict)
                                    { 
                                    string directionFolder = "";
                                    
  
                                    if (cntrDirs > 1) directionFolder = objectFolder + $"Direction {dir.Key}\\";
                                    else directionFolder = animationFolder;
                                    var frms = dir.Value.Frames;
                                    for (int i = 0; i < frms.Count; i++)
                                    {
                                        var frm = frms[i];
                                        Directory.CreateDirectory(directionFolder);
                                        Directory.CreateDirectory($"{frameFolder}{{unsorted}}");
                                        images[frm]?.bitmap.Save($"{directionFolder}{frm}.png");
                                        images[frm]?.bitmap.Save($"{frameFolder}{{unsorted}}\\{imageNumber}.png");
                                        imageNumber++;
                                    }
                                }
                            }
                        }
                        else if(common.Parent.ObjectType==7)
                        {
                            var counter = common.Counters;
                            if (!(counter.DisplayType == 1 || counter.DisplayType == 4 || counter.DisplayType == 50)) continue;
                            foreach (var cntrFrm in counter.Frames)
                            {
                                Directory.CreateDirectory(objectFolder);
                                Directory.CreateDirectory($"{frameFolder}{{unsorted}}");
                                images[cntrFrm]?.bitmap.Save($"{objectFolder}{cntrFrm}.png");
                                images[cntrFrm]?.bitmap.Save($"{frameFolder}{{unsorted}}\\{imageNumber}.png");
                                imageNumber++;
                            }
                        }
                    }
                }
            }
        }
    }
}
