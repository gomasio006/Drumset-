
function PointXZ(x, z) {
	this.x = x;
	this.z = z;
}

PointXZ.prototype.x = null;
PointXZ.prototype.z = null;

PointXZ.prototype.toString = function() {
	return "(x=" + this.x + ", z=" + this.z + ")";
};
PointXZ.prototype.pararell = function(angle, dist, orientation) {
	var dist = orientation > 0 ? dist : -dist;
	var nx = this.x + dist*Math.sin(angle);
	var nz = this.z + dist*Math.cos(angle);
	return new PointXZ(nx, nz);
};

function Stick(sp, ep, w) {

	if (!(sp instanceof PointXZ && ep instanceof PointXZ) || !w)
		return;
	this.st = sp;
	this.ed = ep;
	this.angle = Math.atan((this.ed.z - this.st.z) / (this.st.x - this.ed.x));

	this.width = w;
	this.st2 = this.st.pararell(this.angle, this.width, 1);
	this.ed2 = this.ed.pararell(this.angle, this.width, 1);
};

Stick.prototype.st = null;
Stick.prototype.ed = null;
Stick.prototype.st2 = null;
Stick.prototype.ed2 = null;
Stick.prototype.width = 10;
Stick.prototype.angle = null;

Stick.prototype.toString = function() {
	return "(st=" + this.st + " ed=" + this.ed + ", st2=" + this.st2 + " ed2=" + this.ed2 + ")";
}
