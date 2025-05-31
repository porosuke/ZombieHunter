using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace shooting
{
    public enum SoundType
    {
        outofbullets,
        cut,

        HGshot,
        HGmagrelease,
        HGmagset,
        HGset,

        SGshot,
        SGpomp,
        SGreload,
        SGset,

        ARshot,
        ARmagrelease,
        ARmagset,
        ARrelease,

        SRshot,
        SRcock,
        SRreload,
        SRset,

        damage,
        down,
        care,
        WeaponGet,
        BulletGet,
        Stagein,
        Stageout,
        clear,

        claw,
        zombieShot,
        zombie1_v,
        zombie2_v,
        zombie3_v,
        zombie4_v,
        zombie5_v,
        zombieparty,
        zombieDeath,
        phase2,
        phase3,
        SilentDummy
    }

    public class PlaySound
    {
        private Dictionary<SoundType, BlockSoundCategory> soundToCategory = new Dictionary<SoundType, BlockSoundCategory>()
        {
            { SoundType.claw, BlockSoundCategory.claw },
            { SoundType.damage, BlockSoundCategory.damage },
            { SoundType.zombieShot, BlockSoundCategory.zombie_shot},
            { SoundType.zombie1_v, BlockSoundCategory.zombie_voice },
            { SoundType.zombie2_v, BlockSoundCategory.zombie_voice },
            { SoundType.zombie3_v, BlockSoundCategory.zombie_voice },
            { SoundType.zombie4_v, BlockSoundCategory.zombie_voice },
            { SoundType.zombie5_v, BlockSoundCategory.zombie_voice },
            { SoundType.zombieDeath, BlockSoundCategory.zombie_death }
        };
        private readonly Dictionary<BlockSoundCategory, DateTime> lastPlayTime = new Dictionary<BlockSoundCategory, DateTime>();
        //音被り防止時間
        private readonly TimeSpan minInterval = TimeSpan.FromMilliseconds(200);
        private enum BlockSoundCategory
        {
            claw,
            damage,
            zombie_shot,
            zombie_voice,
            zombie_death,
        }


        public void Init()
        {

            Add(SoundType.outofbullets, Properties.Resources.outofbullets);
            Add(SoundType.cut, Properties.Resources.cut);

            Add(SoundType.HGshot, Properties.Resources.HGshot);
            Add(SoundType.HGmagrelease, Properties.Resources.HGmagrelease);
            Add(SoundType.HGmagset, Properties.Resources.HGmagset);
            Add(SoundType.HGset, Properties.Resources.HGset);

            Add(SoundType.SGshot, Properties.Resources.SGshot);
            Add(SoundType.SGpomp, Properties.Resources.SGpomp);
            Add(SoundType.SGreload, Properties.Resources.SGreload);
            Add(SoundType.SGset, Properties.Resources.SGset);

            Add(SoundType.ARshot, Properties.Resources.ARshot);
            Add(SoundType.ARmagrelease, Properties.Resources.ARmagrelease);
            Add(SoundType.ARmagset, Properties.Resources.ARmagset);
            Add(SoundType.ARrelease, Properties.Resources.ARrelease);

            Add(SoundType.SRshot, Properties.Resources.SRshot);
            Add(SoundType.SRcock, Properties.Resources.SRcock);
            Add(SoundType.SRreload, Properties.Resources.SRreload);
            Add(SoundType.SRset, Properties.Resources.SRset);

            Add(SoundType.damage, Properties.Resources.damage);
            Add(SoundType.down, Properties.Resources.down);
            Add(SoundType.care, Properties.Resources.care);
            Add(SoundType.WeaponGet, Properties.Resources.weapon_get);
            Add(SoundType.BulletGet, Properties.Resources.bullet_get);
            Add(SoundType.Stagein, Properties.Resources.Stagein);
            Add(SoundType.Stageout, Properties.Resources.Stageout);
            Add(SoundType.clear, Properties.Resources.clear);

            Add(SoundType.claw, Properties.Resources.claw);
            Add(SoundType.zombieShot, Properties.Resources.ZombieShot);
            Add(SoundType.zombie1_v, Properties.Resources.zombie1_v);
            Add(SoundType.zombie2_v, Properties.Resources.zombie2_v);
            Add(SoundType.zombie3_v, Properties.Resources.zombie3_v);
            Add(SoundType.zombie4_v, Properties.Resources.zombie4_v);
            Add(SoundType.zombie5_v, Properties.Resources.zombie5_v);
            Add(SoundType.zombieparty, Properties.Resources.zombieparty);
            Add(SoundType.zombieDeath, Properties.Resources.zombie_death);
            Add(SoundType.phase2, Properties.Resources.phase2);
            Add(SoundType.phase3, Properties.Resources.phase3);
            Add(SoundType.SilentDummy, Properties.Resources.SilentDummy);

            Play(SoundType.SilentDummy);
        }
        private bool ShouldSkip(SoundType type, DateTime now)
        {
            if (!soundToCategory.TryGetValue(type, out var category)) return false;

            lock (lastPlayTime)
            {
                if (!lastPlayTime.TryGetValue(category, out var lastTime))
                {
                    return false;
                }
                return (now - lastTime) < minInterval;
            }
        }
        private readonly Dictionary<SoundType, byte[]> soundBuffers = new Dictionary<SoundType, byte[]>();
        public void Add(SoundType type, UnmanagedMemoryStream stream)
        {
            var ms = new MemoryStream();
            stream.CopyTo(ms);
            soundBuffers[type] = ms.ToArray();
        }
        public void Play(SoundType type)
        {
            //存在しない音を鳴らそうとしても無視
            if (!soundBuffers.TryGetValue(type, out var data)) return;

            // 同カテゴリの音が短時間に鳴ろうとしていたらブロック
            if (soundToCategory.TryGetValue(type, out var category))
            {
                if (ShouldSkip(type, DateTime.Now))
                {
                    return;
                }
            }

            Task.Run(() =>
            {
                var now = DateTime.Now;
                if (soundToCategory.TryGetValue(type, out var innerCategory))
                {
                    lock (lastPlayTime)
                    {
                        lastPlayTime[innerCategory] = now;
                    }
                }

                var memoryStream = new MemoryStream(data);
                var reader = new WaveFileReader(memoryStream);
                var pcmStream = WaveFormatConversionStream.CreatePcmStream(reader);
                var outputDevice = new WasapiOut(NAudio.CoreAudioApi.AudioClientShareMode.Shared, true, 20);
                if (type == SoundType.zombieparty || type == SoundType.clear)
                {
                    var loopStream = new LoopStream(pcmStream);
                    outputDevice.Init(loopStream);
                }
                else
                {
                    outputDevice.Init(pcmStream);
                }
                outputDevice.Play();
            });
        }
    }
}
