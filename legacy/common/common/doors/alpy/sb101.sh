sudo killall python3
sudo killall python3
cd /home/astra/common/doors/alpy
python3 comcentr.py   &
python3 sgrd.py &
python3 dms.py  -regim work &
python3 writerlog.py &

python3   aldrv209.py -ch 192.168.0.190  -chsleep 0.001 -find n   & 
#python3  aldrv209.py -ch 192.168.0.7 -chsleep 0.001 -find n  &
