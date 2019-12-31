clear all;
global N;
lambda = 0.35e-6;
N = 2000;
width = 1e-6;
depth = 1e-6;
incidentAngle = deg2rad(45);
x = 0;
z = 100;
pmn = PMN(lambda, width,depth);
    tic
    k = 1;
    ni(1001,1) = 0;
for thetaTemp = -pi/2:pi/N:pi/2               %%% nx = jj;
    thetaTemp*2/pi
    f = frequent(lambda,sin(thetaTemp), 0, 0);
    if(incidentAngle + thetaTemp <=0)
        continue;
    end
    qm = QM(lambda, width, depth, incidentAngle + thetaTemp);
    gs = grooveScatter(lambda,incidentAngle + thetaTemp,width,depth,x,z,pmn,qm);
    k = k+ 1;
    for ii = 1:1001
        xx = (ii - 501) * 0.1e-6;
        zz =0e-6;
        xa(ii) = xx;
        freTemp = frequent(lambda,sin(thetaTemp), -xx*sin(incidentAngle) - zz*cos(incidentAngle),-xx*cos(incidentAngle) + zz*sin(incidentAngle));
        ni(ii,1) = ni(ii,1) + freTemp * gs * cos(thetaTemp)^2 * pi/N;
    end
end
toc
xa = xa'*1e6;
ni = 10*log10((abs(ni).^2)*2*pi*z  /lambda);
figure; plot(ni);
axis([0,1000, -100, 0]);

