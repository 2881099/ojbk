"use strict";

var gulp = require('gulp'),
    clean = require('gulp-clean'),
    glob = require("glob");

var paths = {
    devModule: "../Module/",
    hostModule: "./Module/",
    hostWwwrootModules: "./wwwroot/module/"
};

var modules = loadModules();

gulp.task('clean-module', gulp.series(function () {
    return gulp.src([paths.hostModule + '*', paths.hostWwwrootModules + '*'], { read: false })
    .pipe(clean());
}));

gulp.task('copy-module', gulp.series('clean-module', function (cb) {
	modules.forEach(function (module) {
		console.log(paths.devModule + module.fullName + '/Views/**/*.*');
        gulp.src([paths.devModule + module.fullName + '/Views/**/*.*'], { base: module.fullName })
			.pipe(gulp.dest(paths.hostModule + module.fullName));
        gulp.src(paths.devModule + module.fullName + '/bin/Debug/net6.0/**/' + module.fullName + '.*')
            .pipe(gulp.dest(paths.hostModule + module.fullName));
        gulp.src(paths.devModule + module.fullName + '/appsettings.json')
            .pipe(gulp.dest(paths.hostModule + module.fullName));
        gulp.src(paths.devModule + module.fullName + '/wwwroot/**/*.*')
            .pipe(gulp.dest(paths.hostWwwrootModules + module.name));
    });
    cb();
}));

gulp.task('copy-static', gulp.series(function (cb) {
    modules.forEach(function (module) {
        gulp.src([paths.devModule + module.fullName + '/Views/**/*.*'], { base: module.fullName })
            .pipe(gulp.dest(paths.hostModule + module.fullName));
        gulp.src(paths.devModule + module.fullName + '/wwwroot/**/*.*')
            .pipe(gulp.dest(paths.hostWwwrootModules + module.name));
    });
    cb();
}));

function loadModules() {
	var moduleManifestPaths,
        modules = [];

	moduleManifestPaths = glob.sync(paths.devModule + '*/*.csproj', {});
	moduleManifestPaths.forEach(function (moduleManifestPath) {
        var reg = /\/([^\/]+)\/\1\.csproj/.exec(moduleManifestPath);
		var moduleManifest = {
			name: reg[1],
			fullName: reg[1],
			version: "1.0.0"
		}

		//var exec = require('child_process').exec;
		//var child = exec('echo hello ' + name, function (err, stdout, stderr) {
		//	if (err) throw err;
		//	console.log(stdout);
		//});

		modules.push(moduleManifest);
	});

	return modules;
}