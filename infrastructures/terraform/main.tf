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
  default     = "flex_server"
}

variable "my_ip" {
  description = "Địa chỉ IP của bạn để mở cổng SSH (vd: 203.0.113.5/32)"
  type        = string
  default     = "0.0.0.0/0"
}

# --- Data ---
data "aws_vpc" "flex_vpc" {
  id = var.flex_vpc_id
}

data "aws_subnet_ids" "flex_subnets" {
  vpc_id = data.aws_vpc.flex_vpc.id
}

# --- Security Group ---
resource "aws_security_group" "flex_security_group" {
  name        = "flex_security_group"
  description = "Security Group cho hệ thống"
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

# --- Instance ---
resource "aws_instance" "flex_server" {
  ami                         = var.flex_ami_id
  instance_type               = var.flex_instance_type
  subnet_id                   = data.aws_subnet_ids.flex_subnets.ids[0] # chọn subnet đầu tiên
  vpc_security_group_ids      = [aws_security_group.flex_security_group.id]
  key_name                    = var.flex_key_name

  tags = {
    Name = "Flex-Server"
  }
}

# --- Outputs ---
output "flex_server_details" {
  description = "Thông tin EC2 instance"
  value = {
    public_ip     = aws_instance.flex_server.public_ip
    private_ip    = aws_instance.flex_server.private_ip
    instance_id   = aws_instance.flex_server.id
    instance_type = aws_instance.flex_server.instance_type
  }
}
