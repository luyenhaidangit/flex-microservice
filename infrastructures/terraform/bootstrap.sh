#!/bin/bash

# 🚀 Bootstrap Terraform Backend (S3 + DynamoDB)
set -e

WORK_DIR="terraform/bootstrap"
TFVARS="$WORK_DIR/terraform.tfvars"

echo "Switching to: $WORK_DIR"
cd "$WORK_DIR"

echo "Running: terraform init"
terraform init -input=false

echo "Running: terraform plan"
terraform plan -input=false ${TFVARS:+-var-file=$TFVARS}

read -p "Apply backend setup (S3 + DynamoDB)? (yes/no): " confirm
if [ "$confirm" != "yes" ]; then
  echo "❌ Cancelled."
  exit 0
fi

echo "Running: terraform apply"
terraform apply -input=false -auto-approve ${TFVARS:+-var-file=$TFVARS}

echo "Bootstrap complete!"
