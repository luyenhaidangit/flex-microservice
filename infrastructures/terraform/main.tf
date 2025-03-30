# --- Provider ---
provider "aws" {
  region = "ap-southeast-1"
}

# --- Variables ---
variable "flex_vpc_id" {
  description = "ID của VPC"
  type        = string
  default     = "vpc-04d751b3d7f051427"
}

variable "flex_ami_id" {
  description = "ID của AMI dùng cho EC2"
  type        = string
  default     = "ami-06661384e66f2da0e"
}

variable "flex_instance_type" {
  description = "Loại EC2 instance"
  type        = string
  default     = "t2.micro"
}

variable "flex_key_name" {
  description = "Tên SSH Key Pair dùng để truy cập EC2"
  type        = string
  default     = "linux_server_key"
}

variable "my_ip" {
  description = "Địa chỉ IP của bạn để mở cổng SSH"
  type        = string
  default     = "0.0.0.0/0"
}

variable "enable" {
  description = "Bật hoặc tắt EC2 (stop nếu false)"
  type        = bool
  default     = true
}

# --- Data ---
data "aws_vpc" "flex_vpc" {
  id = var.flex_vpc_id
}

data "aws_subnets" "flex_subnets" {
  filter {
    name   = "vpc-id"
    values = [data.aws_vpc.flex_vpc.id]
  }
}

# --- Security Group ---
resource "aws_security_group" "flex_security_group" {
  name        = "flex_security_group"
  description = "Security Group for system"
  vpc_id      = data.aws_vpc.flex_vpc.id

  ingress {
    from_port   = 22
    to_port     = 22
    protocol    = "tcp"
    cidr_blocks = [var.my_ip]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

# --- EC2 Instance (luôn tạo, không dùng count) ---
resource "aws_instance" "flex_server" {
  ami                    = var.flex_ami_id
  instance_type          = var.flex_instance_type
  subnet_id              = data.aws_subnets.flex_subnets.ids[0]
  vpc_security_group_ids = [aws_security_group.flex_security_group.id]
  key_name               = var.flex_key_name

  tags = {
    Name = "Flex-Server"
  }
}

# --- Stop instance nếu enable = false ---
resource "null_resource" "stop_ec2_when_disabled" {
  count = var.enable ? 0 : 1

  provisioner "local-exec" {
    command = "aws ec2 stop-instances --instance-ids ${aws_instance.flex_server.id} --region ap-southeast-1"
  }

  triggers = {
    always_run = timestamp()
  }

  depends_on = [aws_instance.flex_server]
}

resource "null_resource" "start_ec2_when_enabled" {
  count = var.enable ? 1 : 0

  provisioner "local-exec" {
    command = "aws ec2 start-instances --instance-ids ${aws_instance.flex_server.id} --region ap-southeast-1"
  }

  triggers = {
    always_run = timestamp()
  }

  depends_on = [aws_instance.flex_server]
}

# --- Outputs ---
output "flex_server_details" {
  value = {
    public_ip     = aws_instance.flex_server.public_ip
    private_ip    = aws_instance.flex_server.private_ip
    instance_id   = aws_instance.flex_server.id
    instance_type = aws_instance.flex_server.instance_type
    status        = var.enable ? "enabled" : "disabled"
  }
}
