variable "aws_region" {
  default = "ap-southeast-1"
}

variable "bucket_name" {
  default = "my-terraform-states"
}

variable "dynamodb_table" {
  default = "terraform-locks"
}

variable "environment" {
  default = "bootstrap"
}
