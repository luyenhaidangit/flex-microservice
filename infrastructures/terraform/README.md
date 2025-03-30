rm -rf .terraform terraform.tfstate terraform.tfstate.backup .terraform.lock.hcl
terraform init
terraform apply

terraform import aws_instance.flex_server i-0fd379009c7260b01
terraform import aws_security_group.flex_security_group sg-074bfe1978f3ba94e
