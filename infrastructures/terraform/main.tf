# --- Provider ---
provider "aws" {
  region = "ap-southeast-1"
}

# --- Variables ---
variable "instance_type" {
  default = "t2.micro"
}

# --- Security Group ---
resource "aws_security_group" "ssh_access" {
  name        = "ssh-access"
  description = "Allow SSH from my IP"

  ingress {
    from_port   = 22  # Port SSH
    to_port     = 22
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]  # ⚠️ Nên thay bằng IP của bạn (ví dụ: "123.45.67.89/32")
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]  # Cho phép máy ảo ra internet
  }
}

# --- EC2 Instance ---
resource "aws_instance" "linux_server" {
  ami                    = "ami-06661384e66f2da0e"
  instance_type          = var.instance_type
  vpc_security_group_ids = [aws_security_group.ssh_access.id]
  key_name               = "linux_server_key"

  tags = {
    Name = "Linux-SSH-Server"
  }
}

# --- Outputs ---
output "public_ip" {
  value = aws_instance.linux_server.public_ip  # Đã sửa từ web_server -> linux_server
}
