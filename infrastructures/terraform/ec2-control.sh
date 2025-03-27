#!/bin/bash
INSTANCE_ID="i-0028d68e34338b606"
REGION="ap-southeast-1"

case "$1" in
  start)
    aws ec2 start-instances --instance-ids $INSTANCE_ID --region $REGION
    ;;
  stop)
    aws ec2 stop-instances --instance-ids $INSTANCE_ID --region $REGION
    ;;
  *)
    echo "Usage: $0 {start|stop}"
esac
